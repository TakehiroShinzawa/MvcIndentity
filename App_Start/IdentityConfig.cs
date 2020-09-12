using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MvcIdentity.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace MvcIdentity
{
    public class EmailService : IIdentityMessageService
    {
        readonly System.Net.Configuration.SmtpSection smtpSec =
            (System.Net.Configuration.SmtpSection)
            System.Web.Configuration.WebConfigurationManager.GetWebApplicationSection(
                "system.net/mailSettings/smtp");
        public async Task SendAsync(IdentityMessage message)
        {
            // 電子メールを送信するには、電子メール サービスをここにプラグインします。
            //Console.WriteLine("From:{0}", smtpSec.From);
            //Console.WriteLine("Host:{0}", smtpSec.Network.Host);
            //Console.WriteLine("Port:{0}", smtpSec.Network.Port);
            //Console.WriteLine("UserName:{0}", smtpSec.Network.UserName);
            //Console.WriteLine("Password:{0}", smtpSec.Network.Password);

            //var client = new SmtpClient(smtp)
            //{
            //    Port = port,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    EnableSsl = false,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(userName, password)
            //};
            //今回はSendGridを使用します
            var apiKey = ConfigurationManager.AppSettings["SENDGRID_APIKEY"];
            // SENDGRID_APIKEY
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("MailCheck@ex2p.net", "PK Team"),
                Subject = message.Subject,
                PlainTextContent = "Hello, Email!",
                HtmlContent = message.Body
            };
            msg.AddTo(new EmailAddress(message.Destination, null));
            var response = await client.SendEmailAsync(msg);


            //SMTPの時はこっち
            //var client = new SmtpClient();
            //var mailMessage = new MailMessage(smtpSec.From, message.Destination)
            //{
            //    IsBodyHtml = true,
            //    Subject = message.Subject,
            //    Body = message.Body
            //};
            //return client.SendMailAsync(mailMessage);


        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // テキスト メッセージを送信するための SMS サービスをここにプラグインします。
            //return Task.FromResult(0);
            // Twilio Begin
            var accountSid = ConfigurationManager.AppSettings["SMSAccountIdentification"];
            var authToken = ConfigurationManager.AppSettings["SMSAccountPassword"];
            var fromNumber = ConfigurationManager.AppSettings["SMSAccountFrom"];

            TwilioClient.Init(accountSid, authToken);

            MessageResource result = MessageResource.Create(
                new PhoneNumber(message.Destination),
                from: new PhoneNumber(fromNumber),
                body: message.Body
            );

            ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.TraceInformation(result.Status.ToString());
            ////Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
            // Twilio End

            // ASPSMS Begin 
            // var soapSms = new MvcPWx.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            // soapSms.SendSimpleTextSMS(
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"],
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"],
            //   message.Destination,
            //   System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"],
            //   message.Body);
            // soapSms.Close();
            // return Task.FromResult(0);
            // ASPSMS End

        }
    }

    // このアプリケーションで使用されるアプリケーション ユーザー マネージャーを設定します。UserManager は ASP.NET Identity の中で定義されており、このアプリケーションで使用されます。
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // ユーザー名の検証ロジックを設定します
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // パスワードの検証ロジックを設定します
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // ユーザー ロックアウトの既定値を設定します。
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 2 要素認証プロバイダーを登録します。このアプリケーションでは、Phone and Emails をユーザー検証用コード受け取りのステップとして使用します。
            // 独自のプロバイダーをプログラミングしてここにプラグインできます。
            manager.RegisterTwoFactorProvider("電話コード", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "あなたのセキュリティ コードは {0} です。"
            });
            manager.RegisterTwoFactorProvider("電子メール コード", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "セキュリティ コード",
                BodyFormat = "あなたのセキュリティ コードは {0} です。"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // このアプリケーションで使用されるアプリケーション サインイン マネージャーを構成します。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
    public class ApplicationRoleManager : RoleManager<ApplicationRole, string>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store)
            : base(store)
        {
        }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }
}
