using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Facebook.Controllers
{
    public class AdminController : Controller
    {
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
            return View();
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        public IActionResult OtherUsers()
        {
            return View();
        }
        public IActionResult CreateUser()
        {
            return View();
        }
        public IActionResult Settings()
        {
            return View();
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