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
        public string UserID { get; set; }
        public virtual MyUser User { get; set; }

        [ForeignKey("ID")]
        public int PostID { get; set; }

        public virtual Posts Post { get; set; }
        public bool IsLiked { get; set; }
        public DateTime Date { get; set; }
    }
}
