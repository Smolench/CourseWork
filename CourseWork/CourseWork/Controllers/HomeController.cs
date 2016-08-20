using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseWork.Helpers;
using CourseWork.Models;
using System.Collections;
using System.IO;

namespace CourseWork.Controllers
{
    [RequireHttps]
    public class HomeController : BaseController
    {
        private string TempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public ActionResult Index()
        {
            using (SiteContext db = new SiteContext())
            {
                LuceneSearch<Comment>.AddUpdateLuceneIndex(db.Comments.ToList());
                LuceneSearch<Text>.AddUpdateLuceneIndex(db.Texts.ToList());
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string Search (string input)
        {
            string answer = "Посты:\n" + GetSearchResults<Text>(input);
            return answer + "Комментарии:\n" + GetSearchResults<Comment>(input);
        }

        public string GetSearchResults<T> (string input) where T:new()
        {
            List<T> results = (List<T>)LuceneSearch<T>.GlobalSearch(input);
            string answer = "";
            foreach (var result in results)
            {
                answer += typeof(T).GetProperty("Description").GetValue(result) + "\n";
            }
            return answer;
        }

        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable files)
        {
            foreach (HttpPostedFileBase file in files)
            {
                string filePath = Path.Combine(TempPath, file.FileName);
                System.IO.File.WriteAllBytes(filePath, ReadData(file.InputStream));
            }

            return Json("All files have been successfully stored.");
        }

        private byte[] ReadData(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }
    }
}