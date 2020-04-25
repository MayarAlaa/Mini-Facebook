using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Facebook.Models;
using Facebook.Data;
namespace Pages.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        private readonly SignInManager<MyUser> _signInManager;



        public UserController(UserManager<MyUser> userManager, RoleManager<MyRole> roleManager, SignInManager<MyUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
   
        public IActionResult OtherUsers()
        {
            return View();
        }
        

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userID && u.IsDeleted == false && u.IsBlocked == false);


            if (user == null)
            {
                //return RedirectToAction("index"); //add user id
                return RedirectToAction("Profile", "UserProfile", new { Id = userID });
            }

            return View();


        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordViewModel passView)
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userID);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, passView.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            //return RedirectToAction("index"); //add user id
          return RedirectToAction("Profile", "UserProfile", new { Id = userID });
        }
        public IActionResult Search()
        {
            return View();
        }
        public IActionResult CreatePost()
        {
            return PartialView("CreatePost");
        }
        public IActionResult Post()
        {
            return PartialView("Post");
        }


    }
}