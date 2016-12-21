using Cinema_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinema_Client.Controllers
{
    public class HomeController : Controller
    {

        private CinemaEntities db = new CinemaEntities();
        public ActionResult Index()
        {
            var mOVIES = db.MOVIES.OrderBy(x => x.ID_MOVIE).Where(x=> x.IMAGE != "").ToList();
            //mOVIES.Count;
            //mOVIES.ElementAt
            if (mOVIES == null)
            {
                return HttpNotFound();
            }

            ViewBag.movie = mOVIES;
            ViewBag.movieNo = mOVIES.Count;
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

    }
}