﻿using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MvcIdentity.Models;

namespace MvcIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userName = model.UserName;
            var exist = await UserManager.FindByNameAsync(userName);
            if( exist != null && !UserManager.IsEmailConfirmed(exist.Id)) 
            {
                ModelState.AddModelError("", "メールアドレスが認証されていません。再登録を行ってください");
                return View(model);
            }
            if ( exist == null)
            {// e-mailでリトライ
                exist = await UserManager.FindByEmailAsync(model.UserName);
                if( exist != null)
                {
                    userName = exist.UserName;
                }
            }
            // これは、アカウント ロックアウトの基準となるログイン失敗回数を数えません。
            // パスワード入力失敗回数に基づいてアカウントがロックアウトされるように設定するには、shouldLockout: true に変更してください。
            var result = await SignInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    //直接メールを送信したいがIDをクッキー経由で取得するのでリダイレクトする
                    return RedirectToAction("VerifyCode", new { Provider = ConfigurationManager.AppSettings["TwoFactorMethod"], ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "無効なログイン試行です。");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            if (!await SignInManager.SendTwoFactorCodeAsync(provider))
            {
                return View("Error");
            }
            // ユーザーがユーザー名/パスワードまたは外部ログイン経由でログイン済みであることが必要です。
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 次のコードは、2 要素コードに対するブルート フォース攻撃を防ぎます。
            // ユーザーが誤ったコードを入力した回数が指定の回数に達すると、ユーザー アカウントは
            // 指定の時間が経過するまでロックアウトされます。
            // アカウント ロックアウトの設定は IdentityConfig の中で構成できます。
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "無効なコード。");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName =　model.UserName ?? model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var usera = UserManager.FindByEmail(model.Email);
                    if(usera != null && !UserManager.IsEmailConfirmed(usera.Id))
                    {
                        //メアドが未確認の場合登録しなおす
                        UserManager.Delete(usera);
                        result = await UserManager.CreateAsync(user, model.Password);
                    }
                    /*
                    //メールアドレスの被りでエラーを検査する
                    var b = from r in result.Errors
                            where r.Contains(model.Email)
                            select r;
                    if (b.ToList().Count > 0)
                    {
                        //RAW-SQLのサンプルなので残す
                        //被りアドレスが未確認のものなら、削除して再登録する
                        using (var context = new ApplicationDbContext())
                        {
                            var sql = $"SELECT EmailConfirmed FROM AspNetUsers where Email = @email";
                            var param1 = new SqlParameter("email", model.Email);
                            var dat = context.Database.SqlQuery<ConfirmEmail>(sql, param1);
                            var res = dat.ToList();
                            if (!(res[0].EmailConfirmed))
                            {
                                var param2 = new SqlParameter("email", model.Email);
                                sql = $"DELETE FROM AspNetUsers where Email = @email";
                                var res1 = context.Database.ExecuteSqlCommand(sql, param2);
                                result = await UserManager.CreateAsync(user, model.Password);
                            }
                        }
                    }
                    */
                }
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // アカウント確認とパスワード リセットを有効にする方法の詳細については、https://go.microsoft.com/fwlink/?LinkID=320771 を参照してください
                    // このリンクを含む電子メールを送信します
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "アカウントの確認", "このリンクをクリックすることによってアカウントを確認してください <a href=\"" + callbackUrl + "\">こちら</a>");

                    return RedirectToAction("Index", "Home");
                }



                //}
                AddErrors(result);
            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if( result.Succeeded)
            {
                //AutoLogin
                var user = UserManager.FindById(userId);
                SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                //Enable TwoFactorAuthentication
                //確認後のログインを自動で行うが、次回からは二段認証を行うこととする
                await UserManager.SetTwoFactorEnabledAsync(userId, true);
                return RedirectToAction("Index", "Home");
            }
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null ||　( !string.IsNullOrWhiteSpace( user.UserName) && user.UserName != model.UserName) || 
                    !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // ユーザーが存在しないことや未確認であることを公開しません。
                    return View("ForgotPasswordConfirmation");
                }

                // アカウント確認とパスワード リセットを有効にする方法の詳細については、https://go.microsoft.com/fwlink/?LinkID=320771 を参照してください
                // このリンクを含む電子メールを送信します
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "パスワード", "のリセット <a href=\"" + callbackUrl + "\">こちら</a> をクリックして、パスワードをリセットしてください");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // ここで問題が発生した場合はフォームを再表示します
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // ユーザーが存在しないことを公開しません。
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 外部ログイン プロバイダーへのリダイレクトを要求します
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // ユーザーが既にログインを持っている場合、この外部ログイン プロバイダーを使用してユーザーをサインインします
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // ユーザーがアカウントを持っていない場合、ユーザーにアカウントを作成するよう求めます
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 外部ログイン プロバイダーからユーザーに関する情報を取得します
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region ヘルパー
        // 外部ログインの追加時に XSRF の防止に使用します
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}