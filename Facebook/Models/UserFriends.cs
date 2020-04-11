using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class UserFriends
    {
        [ForeignKey("Id")]
        public string UserId { get; set; }
        public  MyUser User { get; set; }


        [ForeignKey("Id")]
        public string FriendId { get; set; }
        public  MyUser Friend { get; set; }

        public bool IsApproved { get; set; }

        public bool IsDeleted { get; set; }
    }
}
