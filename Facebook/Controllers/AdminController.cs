using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.Json;


using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
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
using System.IO;

namespace Facebook.Controllers
{
    //[Authorize(Roles = "Admin")]
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
        //public IActionResult Index()

        //{
        //    _userManager = userManager;
        //    _roleManager = roleManager;
        //}
  
        public IActionResult Users(string id )
        {

            List<MyUser> users = _userManager.Users.ToList();
            List<MyRole> roles = _roleManager.Roles.ToList();

            ViewData["userID"] = id;


            ViewData["Id"] = new SelectList(_roleManager.Roles.Where(r => r.IsDeleted == false), "Id", "Name");
            ViewBag.Users = users;

            var user = users.Find(u => u.Id == id);

            return View(user);
        }


        public IActionResult Roles()
        {
            List<MyRole> roles = _roleManager.Roles.Where(r => r.IsDeleted == false).ToList();

            //return View(roles);

            ViewBag.Roles = roles;
            var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = _userManager.Users.FirstOrDefault(u => u.Id == adminID && u.IsDeleted == false && u.IsBlocked == false);
            

            return View(admin);


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

        [HttpGet]
        public IActionResult CreateUser ()
        {
            ViewData["Id"] = new SelectList(_roleManager.Roles.Where(r => r.IsDeleted == false),"Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync( MyUser user, string role)
        {

            if (ModelState.IsValid)
            {
                if (user.Gender == 'M')
                {
                    FileStream pic = new FileStream("Images/m.jpeg", FileMode.Open);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await pic.CopyToAsync(ms);
                        user.Image = ms.ToArray();
                        pic.Close();
                    }
                }
                else
                {
                    FileStream pic = new FileStream("Images/f.jpeg", FileMode.Open);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await pic.CopyToAsync(ms);
                        user.Image = ms.ToArray();
                        pic.Close();
                    }
                }


                MyUser Newuser = new MyUser()
                {
                  
                    Id=user.Id,
                    FName = user.FName,
                    LName = user.LName,
                    Email = user.Email,
                    Gender = user.Gender,
                    BDay = user.BDay,
                    BMonth = user.BMonth,
                    BYear = user.BYear,
                    IsBlocked = false,
                    IsDeleted = false,
                    EmailConfirmed = false,
                    PhoneNumber = "",
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0,
                    LockoutEnabled = true,
                    UserName = user.FName,
                    NormalizedUserName = user.FName.ToUpper(),
                    NormalizedEmail = user.Email.ToUpper(),
                    Image = user.Image

                };
                var r = await _userManager.CreateAsync(Newuser, user.PasswordHash);
                if (r.Succeeded)
                    await _userManager.AddToRoleAsync(Newuser, role);
                ViewData["Id"] = new SelectList(_roleManager.Roles, "Name");

                return RedirectToAction("users");
            }
            else
            {
                //ViewData["Id"] = new SelectList(_roleManager.Roles, "Id", "Name");
                ViewData["Id"] = new SelectList(_roleManager.Roles, "Name");


                return View();
            }
        }

        public  IActionResult DataSearching(string searchname) 
        {
        

            List<MyUser> users = _userManager.Users.Where(p => p.FName.Contains(searchname)).ToList();
            //   List<MyRole> roles= _roleManager.Roles.ToList();
            //   ViewBag.role = roles;
            // ViewData["Id"] = new SelectList(_roleManager.Roles, "Id", "Name");
            ViewData["Id"] = new SelectList(_roleManager.Roles, "Name");

            return PartialView(users);
         }

       



        //[HttpGet]
        //public IActionResult ChangePassword()

        //{
        //    var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var admin = _userManager.Users.FirstOrDefault(u => u.Id == adminID && u.IsDeleted == false && u.IsBlocked == false);


        //    if (admin == null)
        //    {
        //        //return RedirectToAction("index"); //add user id
        //        return RedirectToAction("Profile", "UserProfile", new { Id = adminID });
        //    }

        //    return View();


        //}


        //[HttpPost]
        //public async Task<IActionResult> ChangePassword(PasswordViewModel passView)
        //{
        //    var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var admin = await _userManager.FindByIdAsync(adminID);
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(admin);
        //    var result = await _userManager.ResetPasswordAsync(admin, token, passView.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return View();
        //    }

        //   // return RedirectToAction("index"); //add user id
        //    return RedirectToAction("Profile", "UserProfile", new { Id = adminID });
        //}
        public IActionResult Search( string id ,string searchname)
        {
            ViewData["userID"] = id;
            List<MyUser> users = _userManager.Users.Where(w=>w.FName.Contains(searchname)).ToList();

            ViewBag.Users = users;

            var user = users.Find(u => u.Id == id);

            return View(user);
            //return View(users);
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        //public IActionResult Profile()
        //{
        //    return View();
        //}
        //public IActionResult CreatePost()
        //{
        //    return PartialView("CreatePost");
        //}
        //public IActionResult Post()
        //{
        //    return PartialView("Post");
        //}

        public async Task<IActionResult> blockAsync(string id) 
        {
            if (id != null)
            {
                MyUser user = _userManager.Users.Single(u => u.Id == id);
                bool status = user.IsBlocked;
                user.IsBlocked = !user.IsBlocked;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("users");
            }
            return RedirectToAction("users");

        }
      [HttpPost]
        public async Task ChangeRole(string usrid, string role)
        {


            MyUser user =  _userManager.Users.SingleOrDefault(s=>s.Id==usrid);
            bool ex = await _userManager.IsInRoleAsync(user, role);
            if (ex == false)
            {
                var r = await _userManager.AddToRoleAsync(user, role);
                bool re = r.Succeeded;

            }



        }
    }
}