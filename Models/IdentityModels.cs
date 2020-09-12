using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcIdentity.Models
{

    // ApplicationUser クラスにさらにプロパティを追加すると、ユーザーのプロファイル データを追加できます。詳細については、https://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // authenticationType が CookieAuthenticationOptions.AuthenticationType で定義されているものと一致している必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // ここにカスタム ユーザー クレームを追加します
            userIdentity.AddClaim(new Claim(CustomClaimTypes.MyPage, this.MyPage));
            return userIdentity;
        }
        [MaxLength(128)]
        public string MyPage { get; set; }
        [MaxLength(32)]
        public string Twitter { get; set; }
    }


    public class ApplicationRole : IdentityRole
    {
        [Display(Name = "RoleFlag")]
        public int RoleFlags { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("MvcIdentityConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}