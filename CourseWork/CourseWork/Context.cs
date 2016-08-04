using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CourseWork.Models;

namespace CourseWork
{
    class SiteContext : DbContext
    {
        public SiteContext() : base("DefaultConnection")
        { }

        public DbSet<Award> Awards { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Graphic> Graphics { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<User> Users { get; set; }
    }
}