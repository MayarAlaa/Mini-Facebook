using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class MyUser : IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public char Gender { get; set; }
        public string Bio { get; set; }
        public byte[] Photo { get; set; }
        public bool IsBlocked { get; set; }
        public int BDay { get; set; }
        public int BMonth { get; set; }
        public int BYear { get; set; }

        public MyUser():base()
        {

        }

        public MyUser(string userName):base(userName)
        {

        }
    }
}
