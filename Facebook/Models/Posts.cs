using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Posts
    {
        [Key]
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("Id")]
        public string UserId { get; set; }
        public  MyUser User { get; set; }
        public int LikesCount { get; set; }
        public int CommentCount { get; set; }
        public  ICollection<Likes> Likes { get; set; }
        public  ICollection<Comments> Comments { get; set; }
    }
}
