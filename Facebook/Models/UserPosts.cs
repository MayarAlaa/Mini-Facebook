using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class UserPosts
    {
        public MyUser MyUser { get; set; }
        public List<Posts> MyPosts { get; set; }
        public UserPosts()
        {
            MyUser = new MyUser();
            MyPosts = new List<Posts>();
        }
    }
}
