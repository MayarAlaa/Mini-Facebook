using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Comments
    {
        [Key]
        public int CommentId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Id")]
        public string UserId { get; set; }

        public  MyUser User { get; set; }

        [ForeignKey("PostId")]
        public int PostId { get; set; }

        public  Posts Post { get; set; }
    }
}
