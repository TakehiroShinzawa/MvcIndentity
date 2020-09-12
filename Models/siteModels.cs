using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace MvcIdentity.Models
{

    public class MemberInfo
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
    }
    // アクションメソッド Create, Edit 用の Model
    public class RoleModel : ApplicationRole
    {
        [Required]
        [StringLength(
          100,
          ErrorMessage = "{0} は {2} 文字以上",
          MinimumLength = 3)]
        [Display(Name = "RoleName")]
        public new string Name { get; set; }

        //[Display(Name = "RoleFlag")]
        //public int RoleFlags { get; set; }

    }

    // アクションメソッド UserWithRoles, EditRoleAssignment
    //  (各ユーザーのロール一覧表示、ロールのアサイン・削除) 
    // 用の Model
    public class UserWithRoleInfo
    {
        public UserWithRoleInfo()
        {
            UserRoles = new List<RoleInfo>();
        }

        public string UserId { set; get; }
        public string UserName { set; get; }
        public string UserEmail { set; get; }

        public IList<RoleInfo> UserRoles { set; get; }
    }

    public class RoleInfo
    {
        public string RoleName { set; get; }
        public bool IsInThisRole { set; get; }
    }

    public class BusinessMode
    {
        public BusinessMode()
        {
            Authors = new List<Author>();
            Operrators = new List<ApplicationUser>();
            MemberInfos = new List<MemberInfo>();
        }
        public string State;
        public IList<Author> Authors { set; get; }
        public IList<MemberInfo> MemberInfos { set; get; }
        public IList<ApplicationUser> Operrators { set; get; }

    }
    /// <summary>
    /// ユーザーのカスタムクレーム(追加プロパティー)のアクセスID
    /// </summary>
    public static class CustomClaimTypes
    {
        public const string TenantId = "http://example.org/claims/tenantid";
        public const string MyPage = "MyPageID";
    }

}