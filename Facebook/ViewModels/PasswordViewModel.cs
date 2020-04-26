using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models;

namespace Facebook.ViewModels
{
    public class PasswordViewModel
    {
        [DataType(DataType.Password), Required(ErrorMessage = "Old Password Required")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "New Password Required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Confirm Password Required")]
        [Compare("NewPassword",ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

  //      public MyUser myUser { get; set; }
    }

    
}
