using Cinema_Client;
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
            var mOVIES = db.MOVIES.OrderBy(x => x.ID_MOVIE).Where(x => x.IMAGE != "").ToList();

            if (mOVIES == null)
            {
                return HttpNotFound();
            }

            ViewBag.movie = mOVIES;
            ViewBag.movieNo = mOVIES.Count;

            List<string> LinkList = new List<string>();
            List<string> IdList = new List<string>();
            foreach (var elem in mOVIES)
            {
                LinkList.Add(elem.IMAGE);
                IdList.Add(elem.ID_MOVIE);
            }
            ViewBag.Link = LinkList;
            ViewBag.Id = IdList;

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