using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pages.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult OtherUsers()
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