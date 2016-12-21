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
    public class PriceListController : Controller
    {
        private CinemaEntities db = new CinemaEntities();

        // GET: priceList
        public ActionResult Index()
        {
            return View(db.TICKETS.ToList());
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
