using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Facebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Facebook.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        public AdminController(UserManager<MyUser> userManager, RoleManager<MyRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
  
        public IActionResult Users(string id )
        {

            List<MyUser> users = _userManager.Users.ToList();
            List<MyRole> roles = _roleManager.Roles.ToList();
     

            
            ViewData["Id"] = new SelectList(_roleManager.Roles, "Id", "Name");

            return View(users);
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
        public IActionResult OtherUsers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser ()
        {
            ViewData["Id"] = new SelectList(_roleManager.Roles,"Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync( MyUser user, string role)
        {

            if (ModelState.IsValid)
            {

                
               
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
                    // PasswordHash = user.PasswordHash






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

        public IActionResult Settings()
        {
            return View();
        }
        public IActionResult Search( string id ,string searchname)
        {
            List<MyUser> users = _userManager.Users.Where(w=>w.FName.Contains(searchname)).ToList();
            return View(users);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
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