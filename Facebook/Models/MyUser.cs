using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class MyUser : IdentityUser
    {
        [MinLength(3)]
        public string FName { get; set; }

        [MinLength(3)]
        public string LName { get; set; }
        public char Gender { get; set; }
        public string Bio { get; set; }
        public byte[] Image { get; set; }
        public bool IsBlocked { get; set; }

        [Range(1,31)]
        public int BDay { get; set; }

        [Range(1, 12)]
        public int BMonth { get; set; }

        [Range(1950, 2005)]
        public int BYear { get; set; }

        public bool IsDeleted { get; set; }

        
        public  ICollection<Likes> Likes { get; set; }

        public  ICollection<UserFriends> Users { get; set; }
        public  ICollection<UserFriends> Friends { get; set; }

        public MyUser():base()
        {

        }

        public MyUser(string userName):base(userName)
        {

        }
        
    }
}
