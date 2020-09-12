using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcIdentity.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MvcIdentity.Controllers
{
    public class RolesController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext().
                    GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext().
                    Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(string[] roll, string UserId)
        {
            //var req = HttpContext.Request;
            //var itm = req.Params;
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    //指定のユーザーから全ての権限を剥奪する

                    await UserManager.RemoveFromRolesAsync(UserId, (UserManager.GetRoles(UserId)).ToArray());
                    var roles = new List<string>(roll);
                    roles.RemoveAll(s => s.Contains("false"));
                    var roleArray = roles.ToArray();
                    if (roleArray.Length > 0)
                        await UserManager.AddToRolesAsync(UserId, roleArray);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ViewBag.ResultMessage = ex.Message;
                }
            }
            TempData["ResultMessage"] = "権限の更新が成功しました";
            return Redirect("UserWithRoles");
        }

        //
        // GET: /Roles/
        public ActionResult Index()
        {
            var roles = RoleManager.Roles.OrderBy(m => m.Name);
            return View(roles);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                // ユーザーが入力したロール名を model.Role から取得し
                // て ApplicationRole　を生成
                var role = new ApplicationRole { Name = model.Name ,
                RoleFlags = model.RoleFlags

                };

                //　上の ApplicationRole から新規ロールを作成・登録
                var result = RoleManager.Create(role);

                if (result.Succeeded)
                {
                    // 登録に成功したら Roles/Index にリダイレクト
                    return RedirectToAction("Index", "Roles");
                }

                // result.Succeeded が false の場合 ModelSate にエ
                // ラー情報を追加しないとエラーメッセージが出ない。
                // AccountController と同様に AddErrors メソッドを
                // 定義して利用（一番下に定義あり）
                AddErrors(result);
            }
             return View(model);
        }

        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            //var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Roles/Delete/5
        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManageUserRoles()
        {
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            //ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            //var account = new AccountController();
            var user = UserManager.FindByName(UserName);
            if( user == null)
            {
                user = UserManager.FindByEmail(UserName);
            }
            if( user != null)
            {
            UserManager.AddToRole(user.Id, RoleName);
            
            ViewBag.ResultMessage = $"Grant roll to {UserName} successfully !";
            
            // prepopulat roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;   

            return View("ManageUserRoles");

            }
            else
            {
                ViewBag.ResultMessage = "User not found.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {            
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                //ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //var account = new AccountController();

                var user = UserManager.FindByName(UserName);
                if (user == null)
                {
                    user = UserManager.FindByEmail(UserName);
                }
                if (user != null) { 

                    ViewBag.RolesForThisUser = UserManager.GetRoles(user.Id);
                }
                // prepopulat roles for the view dropdown
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;            
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            //var account = new AccountController();
            //ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();


            var user = UserManager.FindByName(UserName);
            if (user == null)
            {
                user = UserManager.FindByEmail(UserName);
            }
            if (user != null)
            {
                if (UserManager.IsInRole(user.Id, RoleName))
                {
                    UserManager.RemoveFromRole(user.Id, RoleName);
                    ViewBag.ResultMessage = "Role removed from this user successfully !";
                }
                else
                {
                    ViewBag.ResultMessage = "This user doesn't belong to selected role.";
                }
                // prepopulat roles for the view dropdown
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                return View("ManageUserRoles");
            }
            return Redirect("ManageUserRoles");
        }
        public async Task<ActionResult> UserWithRoles()
        {
            var model = new List<UserWithRoleInfo>();

            // ToList() を付与してここで DB からデータを取得して
            // DataReader を閉じておかないと、下の IsInRole メソッド
            // でエラーになるので注意
            var users = UserManager.Users.
                        OrderBy(user => user.UserName).ToList();
            var roles = RoleManager.Roles.
                        OrderBy(role => role.Name).ToList();

            foreach (ApplicationUser user in users)
            {
                UserWithRoleInfo info = new UserWithRoleInfo();
                info.UserRoles = new List<RoleInfo>();
                info.UserId = user.Id;
                info.UserName = user.UserName;
                info.UserEmail = user.Email;

                foreach (ApplicationRole role in roles)
                {
                    RoleInfo roleInfo = new RoleInfo();
                    roleInfo.RoleName = role.Name;
                    roleInfo.IsInThisRole = await
                        UserManager.IsInRoleAsync(user.Id, role.Name);
                    info.UserRoles.Add(roleInfo);
                }
                model.Add(info);
            }
            //ViewBagにロール名を詰め込む
            ViewBag.roles = roles;
            return View(model);
        }

        // GET: Roles/EditRoleAssignment/Id
        // 指定 Id のユーザーのロールへのアサインの編集
        // Model は上に定義した UserWithRoleInfo クラス
        public async Task<ActionResult>
                            EditRoleAssignment(string UserId)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(
                                    HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return HttpNotFound();
            }

            UserWithRoleInfo model = new UserWithRoleInfo();
            model.UserId = user.Id;
            model.UserName = user.UserName;
            model.UserEmail = user.Email;

            // ToList() を付与しておかないと下の IsInRole メソッド
            // で　DataReader が閉じてないというエラーになる
            var roles = RoleManager.Roles.
                        OrderBy(role => role.Name).ToList();

            foreach (ApplicationRole role in roles)
            {
                RoleInfo roleInfo = new RoleInfo();
                roleInfo.RoleName = role.Name;
                roleInfo.IsInThisRole = await
                    UserManager.IsInRoleAsync(user.Id, role.Name);
                model.UserRoles.Add(roleInfo);
            }

            return View(model);
        }

        // GET: Roles/EditRoleAssignment/Id
        // 指定 Id のユーザーのロールへのアサインの編集
        // Model は上に定義した UserWithRoleInfo クラス
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult>
          EditRoleAssignment(string UserId, UserWithRoleInfo model)
        {
            if (UserId == null)
            {
                return new HttpStatusCodeResult(
                                    HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                IdentityResult result;

                foreach (RoleInfo roleInfo in model.UserRoles)
                {
                    // id のユーザーが roleInfo.RoleName のロールに属して
                    // いるか否か。以下でその情報が必要。
                    bool isInRole = await
                      UserManager.IsInRoleAsync(UserId, roleInfo.RoleName);

                    // roleInfo.IsInThisRole には編集画面でロールのチェッ
                    // クボックスのチェック結果が格納されている
                    if (roleInfo.IsInThisRole)
                    {
                        // チェックが入っていた場合

                        // 既にロールにアサイン済みのユーザーを AddToRole
                        // するとエラーになるので以下の判定が必要
                        if (isInRole == false)
                        {
                            result = await UserManager.
                                     AddToRoleAsync(UserId, roleInfo.RoleName);
                            if (!result.Succeeded)
                            {
                                AddErrors(result);
                                return View(model);
                            }
                        }
                    }
                    else
                    {
                        // チェックが入っていなかった場合

                        // ロールにアサインされてないユーザーを
                        // RemoveFromRole するとエラーになるので以下の
                        // 判定が必要
                        if (isInRole == true)
                        {
                            result = await UserManager.
                                     RemoveFromRoleAsync(UserId, roleInfo.RoleName);
                            if (!result.Succeeded)
                            {
                                AddErrors(result);
                                return View(model);
                            }
                        }
                    }
                }

                // 編集に成功したら Roles/UserWithRoles にリダイレクト
                return RedirectToAction("UserWithRoles", "Roles");
            }

            // 編集に失敗した場合、編集画面を再描画
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
