using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcIdentity.Models
{
    public class Author
    {
        public int Id { get; set; }

        [DisplayName("氏名")]
        public string Name { get; set; }

        //[DisplayName("住所")]
        //public string Address { get; set; }


        [DisplayName("メールアドレス")]
        public string Email { get; set; }

        [DisplayName("生年月日")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime? Birth { get; set; }

        [DisplayName("記事")]
        public virtual ICollection<Article> Articles { get; set; }

        [NotMapped]
        [DisplayName("生年月日")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public string BirthStr
        {
            get 
            {
                if (Birth == null)
                    return ("");
                return Birth?.ToString("yyyy-MM-dd"); 
            }
        }
    }
}