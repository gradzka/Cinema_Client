using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema_Client;

namespace Cinema_Client.Controllers
{
    public class MoviesController : Controller
    {
        private CinemaEntities db = new CinemaEntities();

        // GET: Movies
        public ActionResult Index()
        {
            return View(db.MOVIES.ToList());
        }

        // GET: Movies/Details/5
        public ActionResult Details(string id) //Wiecej
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MOVIES mOVIES = db.MOVIES.Find(id);

            if (mOVIES == null)
            {
                return HttpNotFound();
            }
            return View(mOVIES);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
