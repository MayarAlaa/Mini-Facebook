using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Friends
    {
        [ForeignKey("Id")]
        public string UserID { get; set; }
        public virtual MyUser User { get; set; }

        [ForeignKey("Id")]
        public string FriendID { get; set; }
        public virtual MyUser MyUser { get; set; }

        public bool IsApproved { get; set; }
    }
}
