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
    public class ProgramController : Controller
    {
        private CinemaEntities db = new CinemaEntities();

        // GET: Program
        public ActionResult Index()
        {
            var pROGRAM = db.PROGRAM.Include(p => p.HALLS).Include(p => p.MOVIES).OrderBy(x=>x.DATE).ThenBy(x=>x.TIME);
            return View(pROGRAM.ToList());
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
