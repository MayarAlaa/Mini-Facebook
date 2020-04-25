﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Facebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.IO;
using System.IO.Pipelines;

namespace Facebook.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<MyUser> userManager,
            SignInManager<MyUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

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
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/User/Index");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new MyUser {FName=Input.FName,LName=Input.LName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    Gender=Input.Gender,
                    BDay=Input.BDay,BMonth=Input.BMonth,BYear=Input.BYear
                };

                if(user.Gender=='M')
                {
                    FileStream pic = new FileStream("D:/ITI/MVC/MVC_Project/Project/Facebook/Images/m.jpeg",FileMode.Open);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await pic.CopyToAsync(ms);
                        user.Image = ms.ToArray();
                        pic.Close();
                    }
                }
                else
                {
                    FileStream pic = new FileStream("D:/ITI/MVC/MVC_Project/Project/Facebook/Images/f.jpeg", FileMode.Open);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await pic.CopyToAsync(ms);
                        user.Image = ms.ToArray();
                        pic.Close();
                    }
                }
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var usr= await _userManager.FindByEmailAsync(Input.Email);
                    _logger.LogInformation("User created a new account with password.");

                    #region Email Confirmation
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    //}
                    //else
                    //{ 
                    #endregion
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "User",user,user.FName);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
