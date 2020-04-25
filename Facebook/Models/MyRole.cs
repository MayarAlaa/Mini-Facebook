using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class MyRole:IdentityRole
    {
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public MyRole():base()
        {

        }

        public MyRole(string roleName):base(roleName)
        {

        }

        public MyRole(string roleName, string description)
        {
            Name = roleName;
            Description = description;
            IsDeleted = false;
            
        }
    }
}
