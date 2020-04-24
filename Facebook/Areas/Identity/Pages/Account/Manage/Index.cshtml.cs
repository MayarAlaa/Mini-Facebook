using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Facebook.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;

        public IndexModel(
            UserManager<MyUser> userManager,
            SignInManager<MyUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [MinLength(3)]
            [Display(Name = "First Name")]
            public string FName { get; set; }

            [Required]
            [MinLength(3)]
            [Display(Name = "Last Name")]
            public string LName { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public char Gender { get; set; }

            [Required]
            [Display(Name = "Day")]
            [Range(1, 31)]
            public int BDay { get; set; }

            [Required]
            [Range(1, 12)]
            [Display(Name = "Month")]
            public int BMonth { get; set; }

            [Required]
            [Range(1950, 2005)]
            [Display(Name = "Year")]
            public int BYear { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(MyUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FName = user.FName,
                LName = user.LName,
                Gender = user.Gender,
                BDay = user.BDay,
                BMonth = user.BMonth,
                BYear = user.BYear,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (Input.FName != user.FName)
            {
                user.FName = Input.FName;
            }

            if (Input.LName != user.LName)
            {
                user.LName = Input.LName;
            }

            if (Input.Gender != user.Gender)
            {
                user.Gender = Input.Gender;
            }

            if (Input.BDay != user.BDay)
            {
                user.BDay = Input.BDay;
            }

            if (Input.BMonth != user.BMonth)
            {
                user.BMonth = Input.BMonth;
            }

            if (Input.BYear != user.BYear)
            {
                user.BYear = Input.BYear;
            }
            await _userManager.UpdateAsync(user);



            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
