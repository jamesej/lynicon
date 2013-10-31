using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Membership;

namespace Lynicon.Base.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        public Guid ItemIdentity { get; set; }

        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
