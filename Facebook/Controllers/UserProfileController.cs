using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Facebook.Data;
using Facebook.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
namespace Facebook.Controllers
{
    public class UserProfileController : Controller
    {
        ApplicationDbContext ProfileContext;
        public UserProfileController(ApplicationDbContext con)
        {
            ProfileContext = con;
        }
        
        public IActionResult Profile(string Id)
        {
            #region UserInfo andPosts

            MyUser user = ProfileContext.Users.FirstOrDefault(u => u.Id == Id);
            var UserPosts = (from p in ProfileContext.Posts
                             where p.UserId == user.Id && p.IsDeleted == false
                             select p).OrderByDescending(d => d.Date).ToList();
            ViewBag.MyPosts = UserPosts;

            if (user == null || Id == null)
                return RedirectToAction("Index", "User");
            #endregion
            #region UserFriends and Requests

            ViewBag.MyFriends = (from UF in ProfileContext.Users
                                 where UF.Friends.FirstOrDefault(x => x.UserId == user.Id && x.IsDeleted==false && x.IsApproved==true) != null 
                                 select UF).ToList();

            ViewBag.MyFriendRequests = (from UF in ProfileContext.Users
                                 where UF.Friends.FirstOrDefault(x => x.UserId == user.Id && x.IsDeleted == false && x.IsApproved == false) != null
                                 select UF).ToList();

            #endregion
            #region AdminRegion
            string RoleId = (ProfileContext.Roles.FirstOrDefault(x => x.Name == "Admin" || x.Name == "admin" || x.Name == "ADMIN")).Id;
            IdentityUserRole<string> R1=ProfileContext.UserRoles.FirstOrDefault(x => x.UserId == Id && x.RoleId == RoleId);
            if (R1 != null) //=>HeIsAnAdmin
            {
                return View("AdminProfile",user);
            }
            #endregion


            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateImg(MyUser user,List<IFormFile> Image)
        {
            MyUser newUser = ProfileContext.Users.FirstOrDefault(u => u.Id == user.Id);

            foreach (var item in Image )
            {
                if(item.Length>0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await item.CopyToAsync(stream);
                        newUser.Image = stream.ToArray();
                    }
                }
            }
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = newUser.Id });

        }

        [HttpPost]
        public IActionResult UpdateInfo(MyUser user)
        {
            #region UpdateUserInfo
            MyUser newUser = ProfileContext.Users.FirstOrDefault(u => u.Id == user.Id);
            if (ModelState.IsValid)
            {
                newUser.FName = user.FName;
                newUser.LName = user.LName;
                newUser.Bio = user.Bio;
                newUser.BDay = user.BDay;
                newUser.BYear = user.BYear;
                newUser.BMonth = user.BMonth;
                ProfileContext.SaveChanges();
                return RedirectToAction("Profile", "UserProfile", new { Id = newUser.Id });
            }
            return RedirectToAction("Profile", "UserProfile", new { Id = newUser.Id });
            #endregion

        }
        #region PostsCRUD
        [HttpPost]
        public IActionResult EditPost(int Id, string Cont)
        {
            #region EditPost
            Posts p = ProfileContext.Posts.FirstOrDefault(post => post.PostId ==Id);
            p.Content = Cont;
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = p.UserId });
            #endregion
        }

       
       public IActionResult EditPostForm(int PId)
        {
            Posts Post = ProfileContext.Posts.FirstOrDefault(Pst => Pst.PostId == PId);
            return View(Post);
        }


        public IActionResult DeletePost(int Id)
        {
            #region DeleteThePost
            Posts p = ProfileContext.Posts.FirstOrDefault(post => post.PostId == Id);
            p.IsDeleted = true;
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = p.UserId });
            #endregion

        }
        [HttpPost]
        public IActionResult AddPost(string Id,string Cont)
        {
            #region InsertPost
            Posts p = new Posts()
            {
                UserId = Id,
                Content = Cont,
                IsDeleted = false,
                Date = DateTime.Now
            };
            ProfileContext.Posts.Add(p);
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = p.UserId });
            #endregion

        }
        #endregion
        #region LikesHandling

        #region Like/Unlike
        //LikeInMyProfile
        public IActionResult LikePost1(int PId, string UId)
        {
            Likes L = ProfileContext.Likes.FirstOrDefault(Lik => Lik.PostId == PId && Lik.UserId == UId);
            Posts P = ProfileContext.Posts.FirstOrDefault(po => po.PostId == PId);
            if (L != null)
            {
                if (L.IsLiked == false)
                {
                    L.IsLiked = true;
                    P.LikesCount = P.LikesCount + 1;

                }
                else if (L.IsLiked == true)
                {
                    L.IsLiked = false;
                    P.LikesCount = P.LikesCount - 1;
                }
                ProfileContext.SaveChanges();
                return RedirectToAction("Profile", "UserProfile", new { Id = UId });

            }
            Likes L1 = new Likes() {
            IsLiked = true,
            PostId = PId,
            IsDeleted = false,
            UserId = UId,
            Date = DateTime.Now
                };
            P.LikesCount = P.LikesCount + 1;
            ProfileContext.Add(L1);

            ProfileContext.SaveChanges();

            return RedirectToAction("Profile", "UserProfile", new { Id = UId });

        }
        //LikeInAnotherProfile
        public IActionResult LikePost2(int PId, string UId,string LikedID)
        {
            Likes L = ProfileContext.Likes.FirstOrDefault(Lik => Lik.PostId == PId && Lik.UserId == LikedID);
            Posts P = ProfileContext.Posts.FirstOrDefault(po => po.PostId == PId);

            if (L != null)
            {
                if (L.IsLiked == false)
                {
                    L.IsLiked = true;
                    P.LikesCount = P.LikesCount + 1;

                }
                else if (L.IsLiked == true)
                {
                    L.IsLiked = false;
                    P.LikesCount = P.LikesCount - 1;
                }
                ProfileContext.SaveChanges();
                return RedirectToAction("AnonFriendORNot", new { Id = LikedID, IDD = UId });

            }
            Likes L1 = new Likes()
            {
                IsLiked = true,
                PostId = PId,
                IsDeleted = false,
                UserId = LikedID,
                Date = DateTime.Now
            };
            ProfileContext.Add(L1);
            P.LikesCount = P.LikesCount + 1;
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = LikedID, IDD = UId });

        }
        #endregion
        #region ShowLikesOnPost
        public IActionResult PostLikes(int PId, string myId)
        {
            Posts ps = ProfileContext.Posts.FirstOrDefault(p => p.PostId == PId);
            ViewData["USID"] = myId;
            var Lik= (from UL in ProfileContext.Users
                                 where UL.Likes.FirstOrDefault(U => U.PostId == PId && U.IsLiked == true) != null
                                 select UL).ToList();
            if(Lik.Count==0)
            {
                return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD =  ps.UserId});
            }

            ViewBag.PostLikes = Lik;
            return View(ps);
        }





        #endregion

        #endregion
        #region CommentsHandling
        #region AddComments
        public IActionResult CommentPost1(int PId, string UId,string Cont)
        {
            Posts P = ProfileContext.Posts.FirstOrDefault(p => p.PostId == PId);
            Comments C = new Comments()
            {
                Content = Cont,
                PostId = PId,
                UserId = UId,
                Date = DateTime.Now,
                IsDeleted = false

            };
            P.CommentCount = P.CommentCount + 1;
            ProfileContext.Add(C);
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = UId });

        }

        public IActionResult CommentPost2(int PId, string UId, string Cont,string CID)
        {
            Posts P = ProfileContext.Posts.FirstOrDefault(p => p.PostId == PId);
            Comments C = new Comments()
            {
                Content = Cont,
                PostId = PId,
                UserId = CID,
                Date = DateTime.Now,
                IsDeleted = false

            };
            P.CommentCount = P.CommentCount + 1;
            ProfileContext.Add(C);
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = CID, IDD = UId });

        }
        #endregion

        #region ShowComments
        public IActionResult PostComments(int PId, string myId)
        {
            Posts ps = ProfileContext.Posts.FirstOrDefault(p => p.PostId == PId);
            ViewData["USID"] = myId;
            var Comm = (from UC in ProfileContext.Users
                       where UC.Comments.FirstOrDefault(U => U.PostId == PId && U.IsDeleted == false) != null
                       select UC).ToList();
                        ViewBag.PostComments = Comm;
            ViewBag.Cont = (from UCC in ProfileContext.Comments
                            where UCC.PostId == PId
                            select UCC).ToList();

            return View(ps);
        }


        #endregion
        #endregion
        #region FriendsHandling
        #region DeleteMyFriend
        public IActionResult RemoveFriend(string Id, string IDD)
        {

            UserFriends UF1 = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == Id && u.UserId == IDD);
            UserFriends UF2 = ProfileContext.UserFriends.FirstOrDefault(u => u.UserId == Id && u.FriendId == IDD);
            ProfileContext.UserFriends.Remove(UF1);
            ProfileContext.UserFriends.Remove(UF2);
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = IDD });
        }
        //ToRedirectToSameViewi'min because i can remove from Two Places
        public IActionResult RemoveFriend2(string Id, string IDD)
        {

            UserFriends UF1 = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == Id && u.UserId == IDD);
            UserFriends UF2 = ProfileContext.UserFriends.FirstOrDefault(u => u.UserId == Id && u.FriendId == IDD);
            ProfileContext.UserFriends.Remove(UF1);
            ProfileContext.UserFriends.Remove(UF2);
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = IDD, IDD = Id });
        }
        #endregion

        #region AcceptFriend
        ///Accepting In MyProffffile
        public IActionResult AcceptFriend(string Id,string  myId)
        {
            //IfHeWasAnOldFriend
            UserFriends F1 = ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == myId && x.IsApproved == false && x.IsDeleted == false);
            UserFriends F2 = ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == myId && x.IsApproved == false && x.IsDeleted == false);
            if (F1 != null && F2 != null)
            {
                F1.IsDeleted = false;
                F2.IsDeleted = false;
                ProfileContext.SaveChanges();
                return RedirectToAction("Profile", "UserProfile", new { Id = myId });

            }

            UserFriends UF = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == Id && u.UserId == myId);
            UF.IsApproved = true;
            UserFriends UF2 = new UserFriends()
            {
                UserId = Id,
                FriendId = myId,
                IsDeleted = false,
                IsApproved=true
      
            };
            ProfileContext.UserFriends.Add(UF2);
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = myId });
        }
        ///Accepting In anotherProfile
        public IActionResult AcceptFriend2(string Id, string myId)
        {
            //IfHeWasAnOldFriend
            UserFriends F1 = ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == myId && x.IsApproved == false && x.IsDeleted == false);
            UserFriends F2 = ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == myId && x.IsApproved == false && x.IsDeleted == false);
            if (F1 != null && F2 != null)
            {
                F1.IsDeleted = false;
                F2.IsDeleted = false;
                ProfileContext.SaveChanges();
                return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });

            }

            UserFriends UF = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == Id && u.UserId == myId);
            UF.IsApproved = true;
            UserFriends UF2 = new UserFriends()
            {
                UserId = Id,
                FriendId = myId,
                IsDeleted = false,
                IsApproved = true

            };
            ProfileContext.UserFriends.Add(UF2);
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });
        }
        #endregion

        #region Ignoring Recived FriendRequest
        public IActionResult CancelRecivedFriend(string Id,string myId)
        {
            UserFriends F1 = ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == myId && x.IsApproved == false && x.IsDeleted == false);
            UserFriends F2 = ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == myId && x.IsApproved == false && x.IsDeleted == false);

            if(F1!=null && F2!=null)
            {
                F1.IsDeleted = true;
                F2.IsDeleted = true;
                ProfileContext.SaveChanges();
                return RedirectToAction("Profile", "UserProfile", new { Id = myId });

            }
            UserFriends UF = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == Id&&u.UserId==myId);
            ProfileContext.Remove(UF);
            ProfileContext.SaveChanges();
            return RedirectToAction("Profile", "UserProfile", new { Id = myId });

        }
        public IActionResult CancelRecivedFriend2(string Id, string myId)
        {
            UserFriends F1 = ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == myId && x.IsApproved == false && x.IsDeleted == false);
            UserFriends F2 = ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == myId && x.IsApproved == false && x.IsDeleted == false);

            if (F1 != null && F2 != null)
            {
                F1.IsDeleted = true;
                F2.IsDeleted = true;
                ProfileContext.SaveChanges();
                return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });

            }
            UserFriends UF = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == Id && u.UserId == myId);
            ProfileContext.Remove(UF);
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });

        }


        #endregion

        #region Cancel Sent Friend Request
        public IActionResult CancelSentFriend(string Id,string myId)
        {
            ///IfUserWas An OldFriend
            UserFriends F1 = ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == myId && x.IsApproved == false && x.IsDeleted == false);
            UserFriends F2 = ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == myId && x.IsApproved == false && x.IsDeleted == false);
            if (F1!=null && F2 != null)
            {
                F1.IsDeleted = true;
                F2.IsDeleted = true;
                ProfileContext.SaveChanges();
                return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });
            }
            ///If he was not an old Friend
            UserFriends UF = ProfileContext.UserFriends.FirstOrDefault(u => u.FriendId == myId && u.UserId==Id);
            ProfileContext.Remove(UF);
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });

        }



        #endregion


        #endregion
        #region GoingToAnotherProfile
        #region GoToPendingProfiles 
        public IActionResult GoToPendingUserProfile(string Id,string myId)
        {
            #region UserInfo andPosts
            ViewData["IDD"] = myId;
            //ForBegining Issues
            ViewData["UserID"] = myId;
            MyUser user = ProfileContext.Users.FirstOrDefault(u => u.Id == Id && u.IsBlocked == false);
            var UserPosts = (from p in ProfileContext.Posts
                             where p.UserId == user.Id && p.IsDeleted == false
                             select p).OrderByDescending(d => d.Date).ToList();
            ViewBag.MyPosts = UserPosts;

            ViewBag.MyPendingFriends = (from UF in ProfileContext.Users
                                 where UF.Friends.FirstOrDefault(x => x.UserId == user.Id && x.IsDeleted == false && x.IsApproved == true) != null
                                 select UF).ToList();
            if (user == null /*|| id == null*/)
                return RedirectToAction("Index", "User");
            return View("PendingUserProfile",user);
            #endregion

        }
        #endregion
        #region FriendProfile
        public IActionResult GoToFriendProfile(string Id,string myID)
        {
            #region UserInfo andPosts
            ViewData["myID"] = myID;

            MyUser user = ProfileContext.Users.FirstOrDefault(u => u.Id == Id && u.IsBlocked == false);
            var UserPosts = (from p in ProfileContext.Posts
                             where p.UserId == user.Id && p.IsDeleted == false
                             select p).OrderByDescending(d => d.Date).ToList();
            ViewBag.MyPosts = UserPosts;

            ViewBag.MyProvedFriends = (from UF in ProfileContext.Users
                                       where UF.Friends.FirstOrDefault(x => x.UserId == user.Id && x.IsDeleted == false && x.IsApproved == true) != null
                                       select UF).ToList();
            if (user == null /*|| id == null*/)
                return RedirectToAction("Index", "User");
            return View("FriendProfile", user);
            #endregion

        }
        #endregion
        #region Don'tKnowIfheIsFriendOrNot 
        public IActionResult AnonFriendORNot(string Id, string IDD)
        {


            MyUser user = ProfileContext.Users.FirstOrDefault(u => u.Id == IDD && u.IsBlocked == false);
            ViewBag.MyPendingFriends = (from UF in ProfileContext.Users
                                        where UF.Friends.FirstOrDefault(x => x.UserId == user.Id && x.IsDeleted == false && x.IsApproved == true) != null
                                        select UF).ToList();
            var UserPosts = (from p in ProfileContext.Posts
                             where p.UserId == user.Id && p.IsDeleted == false
                             select p).OrderByDescending(d => d.Date).ToList();
            ViewBag.MyPosts = UserPosts;
            ViewData["MyID"] = Id;
            ////////////Here The Part Of AdminProfile
            /////FFFriendAccount
            #region FriendsHandling
            if (((ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == IDD && x.IsApproved == true && x.IsDeleted == false) != null)
                && (ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == IDD && x.IsApproved == true && x.IsDeleted == false) != null)) && Id != IDD)
                return RedirectToAction("GoToFriendProfile", "UserProfile", new { Id = IDD, myID = Id });
            else if (Id == IDD)
                ////////MyAccount
                return RedirectToAction("Profile", "UserProfile", new { Id = IDD });
            ///////AlreadySentFreiendRequestAccount
            else if (((ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == IDD && x.IsApproved == false && x.IsDeleted == false) != null)
        && (ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == IDD && x.IsApproved == false && x.IsDeleted == false) != null)) && Id != IDD)

                return RedirectToAction("GoToPendingUserProfile", "UserProfile", new { Id = IDD, myID = Id });
            /////////He AlreadySent Request
            else if (ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == IDD && x.FriendId == Id && x.IsApproved == false && x.IsDeleted == false) != null)
            {

                return View("AlreadySentRequest", user);
            }
            ///////NotttFriend
            else
                return View("NotAFriendProfile", user);
            #endregion


        }
        #endregion
        #endregion
        #region AddFriend
        public IActionResult AddFriend(string Id, string myId)
        {
            //Id ellli mb3otlooo =>friendId
            //myId eliii hyb3aat =>usrId
            //LwKan mawgod we etmasaaa777
          UserFriends F1=ProfileContext.UserFriends.FirstOrDefault(x => x.UserId == Id && x.FriendId == myId && x.IsDeleted == true);
            UserFriends F2 = ProfileContext.UserFriends.FirstOrDefault(x => x.FriendId == Id && x.UserId == myId && x.IsDeleted == true);
            if (F1 != null && F2 != null)

            {
                F1.IsDeleted = false;
                F1.IsApproved = false;
                F2.IsDeleted = false;
                F2.IsApproved = false;
                ProfileContext.SaveChanges();
                return RedirectToAction("AnonFriendORNot", new {Id=myId,IDD=Id});

            }
            //LwAwelMaraAb3atloRequest
            UserFriends UF = new UserFriends();
            UF.FriendId = myId;
            UF.UserId = Id;
            UF.IsDeleted = false;
            UF.IsApproved = false;
            ProfileContext.UserFriends.Add(UF);
            ProfileContext.SaveChanges();
            return RedirectToAction("AnonFriendORNot", new { Id = myId, IDD = Id });

        }
        #endregion

    }

}