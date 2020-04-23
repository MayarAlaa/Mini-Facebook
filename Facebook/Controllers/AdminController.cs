using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Facebook.Data;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Facebook.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        private readonly SignInManager<MyUser> _signInManager;



        public AdminController(UserManager<MyUser> userManager, RoleManager<MyRole> roleManager, SignInManager<MyUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }


        public IActionResult Roles()
        {
            List<MyRole> roles = _roleManager.Roles.Where(r => r.IsDeleted == false).ToList();

            return View(roles);
        }


        public IActionResult CreateRole()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole([Bind("Name,Description")] RoleViewModel myRole)
        {
            if (ModelState.IsValid)
            {
                var deletedRole = _roleManager.Roles.FirstOrDefault(r => r.Name == myRole.Name && r.IsDeleted == true);

                //create the role only if it doesn't exist in DB or exists in DB but was deleted.
                if (deletedRole != null)
                {
                    deletedRole.Description = myRole.Description;
                    deletedRole.IsDeleted = false;
                    await _roleManager.UpdateAsync(deletedRole);
                    return RedirectToAction("Roles");
                }
                else if (!await _roleManager.RoleExistsAsync(myRole.Name))
                {
                    await _roleManager.CreateAsync(new MyRole(myRole.Name, myRole.Description));
                    return RedirectToAction("Roles");
                }
            }
            return RedirectToAction("Roles");
        }



        public IActionResult EditRole(string id)
        {
            var role = _roleManager.Roles.FirstOrDefault(r => r.Id == id && r.IsDeleted == false);

            if (role == null)
            {
                return RedirectToAction("Roles");
            }


            var roleView = new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description

            };

            return View(roleView);


        }


        [HttpPost]
        public async Task<IActionResult> EditRole([Bind("Id,Name,Description")] RoleViewModel myRole)
        {
            if (!ModelState.IsValid)
            {
                return View(myRole);
            }
            
            var role = _roleManager.Roles.FirstOrDefault(r => r.Id == myRole.Id);

            if (role == null)
            {
                RedirectToAction("Roles");
            }


            role.Name = myRole.Name;
            role.NormalizedName = myRole.Name.ToUpper();
            role.Description = myRole.Description;
            await _roleManager.UpdateAsync(role);

            return RedirectToAction("Roles");




        }


        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = _roleManager.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return RedirectToAction("Roles");
            }


            //change roles of users with deleted role to role User

            var usersWithDeletedRoles = await _userManager.GetUsersInRoleAsync(role.NormalizedName);

            if (usersWithDeletedRoles.Count > 0)
            {
                foreach (var user in usersWithDeletedRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                    await _userManager.AddToRoleAsync(user, "user");
                }
            }

            role.IsDeleted = true;

            await _roleManager.UpdateAsync(role);

            return RedirectToAction("Roles");


        }
        public IActionResult OtherUsers()
        {
            return View();
        }
        public IActionResult CreateUser()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _userManager.Users.FirstOrDefault(u => u.Id == adminID && u.IsDeleted == false && u.IsBlocked == false);


            if (admin == null)
            {
                //return RedirectToAction("index"); //add user id
                return RedirectToAction("Profile", "UserProfile", new { Id = adminID });
            }

            return View();


        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordViewModel passView)
        {
            var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await _userManager.FindByIdAsync(adminID);
            var token = await _userManager.GeneratePasswordResetTokenAsync(admin);
            var result = await _userManager.ResetPasswordAsync(admin, token, passView.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

           // return RedirectToAction("index"); //add user id
            return RedirectToAction("Profile", "UserProfile", new { Id = adminID });
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