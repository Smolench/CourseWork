using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseWork.Models
{
    public class Award
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }

        public ICollection<User> Users { get; set; }
        public Award()
        {
            Users = new List<User>();
        }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }

        public int? PageId { get; set; }
        public Page Page { get; set; }
    }

    public class Content
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int TemplatePart { get; set; }
        public int OrderInTemplatePart { get; set; }

        public int? PageId { get; set; }
        public Page Page { get; set; }
        public ICollection<Graphic> Graphics { get; set; }
        public ICollection<Picture> Pictures { get; set; }
        public ICollection<Text> Texts { get; set; }
        public ICollection<Video> Videos { get; set; }
        public Content()
        {
            Graphics = new List<Graphic>();
            Pictures = new List<Picture>();
            Texts = new List<Text>();
            Videos = new List<Video>();
        }
    }

    public class Graphic
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Type { get; set; }

        public int? ContentId { get; set; }
        public Content Content { get; set; }
    }

    public class Page
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Template { get; set; }

        public int? SiteId { get; set; }
        public Site Site { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Page()
        {
            Comments = new List<Comment>();
        }
    }

    public class Picture
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Size { get; set; }

        public int? ContentId { get; set; }
        public Content Content { get; set; }
    }

    public class Site
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string NavigationTemplate { get; set; }
        public int PageCount { get; set; }
        public int Rating { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime EditingTime { get; set; }

        public ICollection<Tag> Tags { get; set; }
        public ICollection<Page> Pages { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public Site()
        {
            Tags = new List<Tag>();
            Pages = new List<Page>();
        }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Site> Sites { get; set; }
        public Tag()
        {
            Sites = new List<Site>();
        }
    }

    public class Text
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int? ContentId { get; set; }
        public Content Content { get; set; }
    }

    public class Video
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public bool IsLooped { get; set; }

        public int? ContentId { get; set; }
        public Content Content { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<Site> Sites { get; set; }
        public ICollection<Award> Awards { get; set; }
        public User()
        {
            Sites = new List<Site>();
            Awards = new List<Award>();
        }
    }
}