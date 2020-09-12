using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcIdentity.Models
{
    [DisplayColumn("Body")]
    public class Comment
    {
        public int Id { get; set; }

        [DisplayName("����")]
        public string Name { get; set; }

        [DisplayName("�R�����g")]
        [Required]
        public string Body { get; set; }

        [DisplayName("�X�V��")]
        [DisplayFormat(DataFormatString = "{0:yyyy�NMM��dd��}")]
        public DateTime Updated { get; set; }

        public int? ArticleId { get; set; }

        [DisplayName("�L��")]
        public virtual Article Article { get; set; }
    }
}
