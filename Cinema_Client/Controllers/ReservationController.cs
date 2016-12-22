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
    public class Detail
    {
        public string row;
        public string seat_no;
        public TICKETS ticket;

        public Detail(string row, string seat_no, TICKETS ticket)
        {
            this.row = row;
            this.seat_no = seat_no;
            this.ticket = ticket;
        }
    }

    public class ReservationController : Controller
    {
        private CinemaEntities db = new CinemaEntities();

        // GET: Reservation
        public ActionResult Index()
        {
            /*
             * wyszukiwanie - idresrvation i userlogin
             * szczegóły data czas sala movie 2d3d version z program klucz główny idprogram
             * resdetails - siedzienia klucz główny idreservation klucz obcy id ticjet
             * tickets - type cena od 2d3d
             * 
             */
            string login = Session["USER_LOGIN"].ToString();
            var rESERVATIONS = db.RESERVATIONS.Include(r => r.PROGRAM).Include(r => r.USERS).Include(r => r.PROGRAM.MOVIES).Where(r => r.USER_LOGIN == login);
            return View(rESERVATIONS.ToList());
        }

        // GET: Reservation/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RESERVATIONS rESERVATIONS = db.RESERVATIONS.Find(id);
            if (rESERVATIONS == null)
            {
                return HttpNotFound();
            }
            var RDetails= db.RESERVATIONS_DETAILS.Where(r => r.ID_RESERVATION == id).ToList();
            string[] seat;
            List<Detail> details = new List<Detail>();
            Detail one_detail;

            foreach (var item in RDetails)
            {
                seat = item.ID_SEAT.Split('_');
                one_detail = new Detail(seat[0], seat[1], item.TICKETS);
                details.Add(one_detail);
            }

            ViewBag.RDetails = details;

            return View(rESERVATIONS);
        }

        // GET: Reservation/Create
        public ActionResult Create()
        {
            ViewBag.ID_PROGRAM = new SelectList(db.PROGRAM, "ID_PROGRAM", "ID_HALL");
            ViewBag.USER_LOGIN = new SelectList(db.USERS, "USER_LOGIN", "PASSWORD");
            return View();
        }

        // POST: Reservation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_RESERVATION,ID_PROGRAM,USER_LOGIN")] RESERVATIONS rESERVATIONS)
        {
            if (ModelState.IsValid)
            {
                db.RESERVATIONS.Add(rESERVATIONS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_PROGRAM = new SelectList(db.PROGRAM, "ID_PROGRAM", "ID_HALL", rESERVATIONS.ID_PROGRAM);
            ViewBag.USER_LOGIN = new SelectList(db.USERS, "USER_LOGIN", "PASSWORD", rESERVATIONS.USER_LOGIN);
            return View(rESERVATIONS);
        }

        // GET: Reservation/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RESERVATIONS rESERVATIONS = db.RESERVATIONS.Find(id);
            if (rESERVATIONS == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_PROGRAM = new SelectList(db.PROGRAM, "ID_PROGRAM", "ID_HALL", rESERVATIONS.ID_PROGRAM);
            ViewBag.USER_LOGIN = new SelectList(db.USERS, "USER_LOGIN", "PASSWORD", rESERVATIONS.USER_LOGIN);
            return View(rESERVATIONS);
        }

        // POST: Reservation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_RESERVATION,ID_PROGRAM,USER_LOGIN")] RESERVATIONS rESERVATIONS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rESERVATIONS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_PROGRAM = new SelectList(db.PROGRAM, "ID_PROGRAM", "ID_HALL", rESERVATIONS.ID_PROGRAM);
            ViewBag.USER_LOGIN = new SelectList(db.USERS, "USER_LOGIN", "PASSWORD", rESERVATIONS.USER_LOGIN);
            return View(rESERVATIONS);
        }

        // GET: Reservation/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RESERVATIONS rESERVATIONS = db.RESERVATIONS.Find(id);
            if (rESERVATIONS == null)
            {
                return HttpNotFound();
            }
            return View(rESERVATIONS);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            RESERVATIONS rESERVATIONS = db.RESERVATIONS.Find(id);
            db.RESERVATIONS.Remove(rESERVATIONS);
            db.SaveChanges();
            return RedirectToAction("Index");
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
