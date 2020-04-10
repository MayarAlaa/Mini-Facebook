using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Likes
    {
        [ForeignKey("Id")]
        public string UserId { get; set; }
        public  MyUser MyUser { get; set; }

        [ForeignKey("PostId")]
        public int PostId { get; set; }

        public  Posts Post { get; set; }
        public bool IsLiked { get; set; }
        public DateTime Date { get; set; }

        public bool IsDeleted { get; set; }
    }
}
