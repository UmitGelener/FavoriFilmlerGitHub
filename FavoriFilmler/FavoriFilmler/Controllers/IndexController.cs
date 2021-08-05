using FavoriFilmler.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FavoriFilmler.Controllers
{
    public class IndexController : Controller
    {
        FavoriFilmlerEntities db = new FavoriFilmlerEntities();
        // GET: Index
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Navbar()
        {
            return PartialView();
        }
        public ActionResult signin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult signin(FormCollection form)
        {
            Users user = new Users();
            user.Mail = form["mail"].Trim();
            user.Password = form["password"].Trim();
            var control = db.Users.FirstOrDefault(x => x.Mail == user.Mail && x.Password == user.Password);
            if(control != null)
            {
                FormsAuthentication.SetAuthCookie(control.Mail,false);
                Session["id"] = control.ID;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["sifrehata"] = "Kullanıcı adı veya şifre hatalıdır.";
                return View();
            }
            return View();
        }
        [HttpPost]
        public ActionResult signup(FormCollection form)
        {
            Users user = new Users();
            user.Mail = form["mail"].Trim();
            user.Password = form["password"].Trim();
            user.Role = "u";
            var control = db.Users.FirstOrDefault(x => x.Mail == user.Mail);
            if(control == null)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["mailhata"] = "Böyle bir hesap vardır.";
                return RedirectToAction("signin");
            }
            return View();
        }

        [Authorize(Roles = "u")]
        public ActionResult favori()
        {
            if (Session["id"] != null) { 
            var id = Convert.ToInt32(Session["id"]);
            var myQ = db.Favoriler.Where(h => h.User_ID == id).ToList();
            return View(myQ);
            }
            else
            {
                return RedirectToAction("signin");
            }
            return View();
        }

        [Authorize(Roles = "u")]
        public ActionResult deletefavori(int id)
        {
            var du = db.Favoriler.Find(id);
            db.Favoriler.Remove(du);
            db.SaveChanges();
            return RedirectToAction("favori", "Index");
        }

        [HttpPost]
        public ActionResult favori(FormCollection form)
        {
            Favoriler fav = new Favoriler();
           if(Session["id"] != null) { 
            fav.User_ID = Convert.ToInt32(Session["id"]);
            fav.Title = form["title"];
            fav.Year = Convert.ToInt32(form["year"]);
            fav.Imdb_Rating = float.Parse(form["imdb_rating"], CultureInfo.InvariantCulture.NumberFormat); 
            fav.Director = form["director"];
            db.Favoriler.Add(fav);
            db.SaveChanges();
            return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("signin");
            }

           
        }

        [Authorize(Roles = "u")]
        public ActionResult signout()
        {
            Session["id"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("signin");
        }

    }
}