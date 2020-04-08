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
        public int ID { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Id")]
        public string UserID { get; set; }

        public virtual MyUser User { get; set; }

        [ForeignKey("ID")]
        public string PostID { get; set; }

        public virtual Posts Post { get; set; }
    }
}
