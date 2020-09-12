//using MvcModel.Extensions;
using MvcIdentity.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcIdentity.Models
{
    //[CustomValidation(typeof(Member), "CheckMarriedAndEmail")]
    public class Member : IValidatableObject
    {
        public int Id { get; set; }

        [DisplayName("氏名")]
        [Required(ErrorMessage = "{0}は必須です。")]
        [RegularExpression("[^a-zA-Z0-9]*", ErrorMessage = "{0}には半角英数字を含めないでください。")]
        public string Name { get; set; }

        [DisplayName("メールアドレス")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "メールアドレスの形式で入力してください")]
        public string Email { get; set; }

        [DisplayName("メールアドレス（確認）")]
        [NotMapped]
        [Compare("Email", ErrorMessage = "{1}と一致していません。")]
        public string EmailConfirmed { get; set; }

        [DisplayName("生年月日")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        [Required(ErrorMessage = "{0}は必須です。")]
        public DateTime Birth { get; set; }
 
        [NotMapped]
        [DisplayName("生年月日")]
        [UIHint("Calendar")]
        public string BirthDate
        {
            get { return Birth.ToString("yyyy年MM月dd日"); }
        }

        [DisplayName("既婚")]
        public bool Married { get; set; }

        [DisplayName("自己紹介")]
        [DataType(DataType.MultilineText)]
        [StringLength(100, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
        [CustomValidation(typeof(Member), "CheckBlackword")]
        [Blackword(/*"違法,麻薬,毒"*/)]
        public string Memo { get; set; }

        public static ValidationResult CheckBlackword(string memo)
        {
            if ( !string.IsNullOrWhiteSpace(memo))
            {
                string[] list = new string[] { "違法", "麻薬", "毒" };
                foreach (var data in list)
                {
                    if (memo.Contains(data))
                    {
                        return new ValidationResult("NGワードが含まれています。");
                    }
                }
            }
            return ValidationResult.Success;
        }

        //public static ValidationResult CheckMarriedAndEmail(Member m)
        //{
        //    if (m.Married && m.Email == null)
        //    {
        //        return new ValidationResult("既婚者はメールアドレスを入力してください。");
        //    }
        //    return ValidationResult.Success;
        //}

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Married && Email == null)
            {
                yield return new ValidationResult("既婚者はメールアドレスを入力してください。");
                //yield return new ValidationResult("既婚者はメールアドレスを入力してください。", new [] { "Email" });
            }
        }

    }
}