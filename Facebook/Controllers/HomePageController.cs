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
        public IActionResult Index(string id)
        {

            ViewData["ID"] = id;
            List<String> user_friends_ids = new List<String>();
            user_friends_ids.Add(id);
            //user_friends_ids.Add("b");
            user_friends_ids.AddRange(fb.UserFriends.Where(f => f.UserId == id && f.IsApproved == true).Select(f => f.FriendId).ToList());

            List<Posts> post_friend = new List<Posts>();
            
            List<Posts> posts = new List<Posts>();
            for (int i = 0; i < user_friends_ids.Count; i++)
            {
                //user_friends_ids[i]
                post_friend.AddRange(fb.Posts.Where(p => p.UserId == user_friends_ids[i] && p.IsDeleted == false).Include("User").Include("Likes").Include("Comments").ToList());

            }

            ViewBag.Posts = post_friend;

            var user = fb.Users.Find(id);

            return View(user);
        }


        //int id
        public IActionResult Getcomment(int id)
        {
            List<Comments> gcom = new List<Comments>();
            gcom.AddRange(fb.Comments.Where(c => c.IsDeleted == false && c.PostId == id).Include("User").OrderByDescending(c => c.Date).ToList());
            return PartialView(gcom);
        }
        //int id
        public IActionResult Getlike(int id)
        {
            List<Likes> gli = new List<Likes>();
            gli.AddRange(fb.Likes.Where(l => l.IsDeleted == false && l.IsLiked == true && l.PostId == id).Include("User").OrderByDescending(l => l.Date).ToList());
            return PartialView(gli);
        }
        [HttpPost]
        public IActionResult createpost(string Content, string UserId)
        {
            Posts crpost = new Posts();

            if (ModelState.IsValid)
            {
                crpost.IsDeleted = false;
                crpost.Date = DateTime.Now;
                crpost.CommentCount = 0;
                crpost.LikesCount = 0;
                crpost.UserId = UserId;
                crpost.Content = Content;
                fb.Add(crpost);
                fb.SaveChanges();
            }

            //var user = fb.Users.Find(UserId);

            return RedirectToAction("Index","HomePage",new { id = UserId });
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
            return RedirectToAction("Index", "HomePage", new { id = crcom.UserId });
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
            return RedirectToAction("Index", "HomePage", new { id = crlike.UserId });

        }

        public IActionResult UpdatePost([Bind("Content", "PostId", "UserId")] Posts uppost)
        {
            //uppost.UserId uppost.PostId
            uppost = fb.Posts.Where(p => p.UserId == uppost.UserId && p.PostId == uppost.PostId).ToList()[0];


            uppost.Date = DateTime.Now;
            return RedirectToAction("Index", "HomePage", new { id = uppost.UserId });



        }
        public IActionResult UpdateComment([Bind("Content", "PostId", "UserId")] Comments upcom)
        {
            //upcom.UserId upcom.PostId
            upcom = fb.Comments.Where(c => c.UserId == upcom.UserId && c.PostId == upcom.PostId).ToList()[0];


            upcom.Date = DateTime.Now;
            return RedirectToAction("Index", "HomePage", new { id = upcom.UserId });



        }


        public IActionResult DeletePost([Bind("PostId", "UserId")] Posts delpost)
        {
            //delpost.UserId       delpost.PostId
            delpost = fb.Posts.Where(p => p.UserId == delpost.UserId && p.PostId == delpost.PostId).ToList()[0];
            delpost.IsDeleted = true;
            fb.SaveChanges();


            return RedirectToAction("Index", "HomePage", new { id = delpost.UserId });

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