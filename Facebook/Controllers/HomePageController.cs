using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Data;
using Facebook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Controllers
{
    public class HomePageController : Controller
    {
        ApplicationDbContext fb;
        public HomePageController(ApplicationDbContext context)
        {
            fb = context;
        }
        //string id
        public IActionResult Index(MyUser user)
        {

            ViewData["ID"] = user.Id;
            List<String> user_friends_ids = new List<String>();
            user_friends_ids.Add(user.Id);
            //user_friends_ids.Add("b");
            user_friends_ids.AddRange(fb.UserFriends.Where(f => f.UserId == user.Id && f.IsApproved == true).Select(f => f.FriendId).ToList());

            List<Posts> post_friend = new List<Posts>();
            List<Posts> posts = new List<Posts>();
            for (int i = 0; i < user_friends_ids.Count; i++)
            {
                //user_friends_ids[i]
                post_friend.AddRange(fb.Posts.Where(p => p.UserId == user.Id && p.IsDeleted == false).Include("User").Include("Likes").Include("Commments").OrderByDescending(p => p.Date).ToList());

            }
            return View(post_friend);
        }


        //int id
        public IActionResult Getcomment()
        {
            List<Comments> gcom = new List<Comments>();
            gcom.AddRange(fb.Comments.Where(c => c.IsDeleted == false && c.PostId == 5).Include("User").OrderByDescending(c => c.Date).ToList());
            return PartialView(gcom);
        }
        //int id
        public IActionResult Getlike()
        {
            List<Likes> gli = new List<Likes>();
            gli.AddRange(fb.Likes.Where(l => l.IsDeleted == false && l.IsLiked == true && l.PostId == 5).Include("User").OrderByDescending(l => l.Date).ToList());
            return PartialView(gli);
        }
        [HttpPost]
        public IActionResult createpost(Posts crpost)
        {
            if (ModelState.IsValid)
            {

                crpost.IsDeleted = false;
                crpost.Date = DateTime.Now;

                fb.Add(crpost);
                fb.SaveChanges();

            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CreateComment([Bind("Content", "PostId", "UserId")] Comments crcom)
        {
            if (ModelState.IsValid)
            {


                crcom.IsDeleted = false;
                crcom.Date = DateTime.Now;
                fb.Add(crcom);
                fb.SaveChanges();

            }
            return RedirectToAction("Index");
        }
        public IActionResult CreateLike([Bind("PostId", "UserId")] Likes crlike)
        {
            if (ModelState.IsValid)
            {
                var like = fb.Likes.Where(l => l.PostId == crlike.PostId && l.UserId == crlike.UserId).ToList();
                if (like.Count == 0)
                {
                    crlike.IsDeleted = false;
                    crlike.Date = DateTime.Now;
                    crlike.IsLiked = true;
                    fb.Add(crlike);
                    fb.SaveChanges();
                }
                else
                {
                    var l = like[0];
                    if (l.IsLiked)
                    {
                        l.IsLiked = false;
                    }
                    else
                    {
                        l.IsLiked = true;
                    }
                    fb.SaveChanges();
                }


            }
            return RedirectToAction("Index");

        }

        public IActionResult UpdatePost([Bind("Content", "PostId", "UserId")] Posts uppost)
        {
            //uppost.UserId uppost.PostId
            uppost = fb.Posts.Where(p => p.UserId == "a" && p.PostId == 5).ToList()[0];


            uppost.Date = DateTime.Now;
            return RedirectToAction("Index");



        }
        public IActionResult UpdateComment([Bind("Content", "PostId", "UserId")] Comments upcom)
        {
            //upcom.UserId upcom.PostId
            upcom = fb.Comments.Where(c => c.UserId == "a" && c.PostId == 5).ToList()[0];


            upcom.Date = DateTime.Now;
            return RedirectToAction("Index");



        }


        public IActionResult DeletePost([Bind("PostId", "UserId")] Posts delpost)
        {
            //delpost.UserId       delpost.PostId
            delpost = fb.Posts.Where(p => p.UserId == "a" && p.PostId == 5).ToList()[0];
            delpost.IsDeleted = true;
            fb.SaveChanges();


            return RedirectToAction("Index");

        }
        public IActionResult DeleteComment([Bind("PostId", "UserId")] Comments delcom)
        {
            //delcom.UserId     delcom.PostId
            delcom = fb.Comments.Where(u => u.UserId == "a" && u.PostId == 5).ToList()[0];
            delcom.IsDeleted = true;
            fb.SaveChanges();


            return RedirectToAction("Index");
        }

    }

}