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
        public int ID { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool isDeleted { get; set; }

        [ForeignKey("Id")]
        public string UserID { get; set; }
        public virtual MyUser User { get; set; }
    }
}
