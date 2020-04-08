using System;
using System.Collections.Generic;
using System.Text;
using Facebook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyUser,MyRole,string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Likes>().HasKey(like => new { like.UserID, like.PostID });
            builder.Entity<Friends>().HasKey(f => new { f.UserID, f.FriendID });

        }

        public DbSet<Posts> Posts { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Friends> Friends { get; set; }
    }
}
