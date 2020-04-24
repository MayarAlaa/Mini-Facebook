using System;
using System.Collections.Generic;
using System.Text;
using Facebook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyUser, MyRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region UserFriends
            builder.Entity<UserFriends>().HasKey(f => new { f.UserId, f.FriendId });
            builder.Entity<UserFriends>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Users)
                .HasForeignKey(uf => uf.UserId);

            builder.Entity<UserFriends>()
                .HasOne(uf => uf.Friend)
                .WithMany(f => f.Friends)
                .HasForeignKey(uf => uf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion


            #region Comments
            builder.Entity<Comments>(entity => 
                { 
                    entity.HasKey(c => c.CommentId); 
                });

            #endregion


            #region MyUser
            builder.Entity<MyUser>(entity =>
            {
                entity.Property(u => u.FName).HasMaxLength(30);
                entity.Property(u => u.LName).HasMaxLength(30);
            });

            #endregion


            #region Posts
            builder.Entity<Posts>(entity =>
                {
                    entity.HasKey(p => p.PostId);

                });
            #endregion


            #region MyUser-Likes-Posts
            builder.Entity<Likes>().HasKey(up => new { up.UserId, up.PostId });

            //one user in User table is mapped to many users in UserPosts table
            builder.Entity<Likes>()
                .HasOne(up => up.MyUser)
                .WithMany(u => u.Likes)
                .HasForeignKey(up => up.UserId) ;

            //one post in Posts table is mapped to many posts in UserPosts table
            builder.Entity<Likes>()
                .HasOne(up => up.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(up => up.PostId); 
            #endregion


        }

        public DbSet<Posts> Posts { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<UserFriends> UserFriends { get; set; }
    }
}
