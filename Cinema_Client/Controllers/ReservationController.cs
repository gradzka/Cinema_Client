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
            if (Session["USER_LOGIN"] != null)
            {
                string login = Session["USER_LOGIN"].ToString();
                var rESERVATIONS = db.RESERVATIONS.Include(r => r.PROGRAM).Include(r => r.USERS).Include(r => r.PROGRAM.MOVIES).Where(r => r.USER_LOGIN == login);
                return View(rESERVATIONS.ToList());
            }
            else
            {
                return RedirectToAction("", "Home");
            }

        }

        // GET: Reservation/Details/5
        public ActionResult Details(short? id)
        {
            if (Session["USER_LOGIN"] != null)
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
                var RDetails = db.RESERVATIONS_DETAILS.Where(r => r.ID_RESERVATION == id).ToList();
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
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        // GET: Reservation/Create
        public ActionResult Create(string movieId, string programId)
        {
            if (movieId != null)
            {
                List<string> dataArray = new List<string>();
                foreach (var elem in db.PROGRAM.Where(x => x.ID_MOVIE == movieId).ToList())
                {
                    dataArray.Add(elem.DATE.ToShortDateString() + " " + elem.TIME.ToString() +" Sala: " +elem.ID_HALL.ToString());
                }
                
                ViewBag.DateTimeHall = new SelectList(dataArray);

                var MovieTitle = db.MOVIES.Where(x => x.ID_MOVIE == movieId).Select(x => x.TITLE).FirstOrDefault();

                Session["mTitle"] = MovieTitle;
                
                //do stworzenia sali kinowej

            }
            if (programId!=null)
            {
                ViewBag.programId = programId;
                var prId = Int32.Parse(programId);

                var MovieIdentity = db.PROGRAM.Where(x => x.ID_PROGRAM == prId).Select(x=>x.ID_MOVIE).FirstOrDefault();
                var MoId = MovieIdentity.ToString();

                List<string> dataArray = new List<string>();
                foreach (var elem in db.PROGRAM.Where(x => x.ID_MOVIE== MoId).ToList())
                {
                    dataArray.Add(elem.DATE.ToShortDateString() + " " + elem.TIME.ToString() + " Sala: " + elem.ID_HALL.ToString());
                }

                ViewBag.DateTime = new SelectList(dataArray);

                var MovieTitle = db.MOVIES.Where(x => x.ID_MOVIE == MoId).Select(x => x.TITLE).FirstOrDefault();

                Session["mTitle"] = MovieTitle;
            }

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

        [HttpPost]
        public ActionResult Next(string DateTimeHall)
        {
            //Session["date_time_hall"] = DateTimeHall;
            Session["DateTimeHall"] = DateTimeHall;
            var movietitle = Session["mTitle"];
            string[] date_time_hall_Array = DateTimeHall.Split(' ');//3 -hall

            List<string> seats = new List<string>();
            var hall = date_time_hall_Array[3];
            seats =db.SEATS.Where(x => x.ID_HALL == hall).Select(x => x.ID_SEAT).ToList();

            //liczba rzedow - wymiar pionowy
            //liczba siedzien - wymiar poziomy

            int seats_number = seats.Count();
            string last_letter = seats.Last();
            int rows_number =(int)(last_letter[0])- 65 +1;
            int columns_number = seats_number / rows_number;

            //Session["rows_nO"]
            ViewBag.rows_No = rows_number;
            ViewBag.columns_No = columns_number;
            ViewBag.screen = columns_number - 4;
            ViewBag.lastletter = last_letter[0];

            //Bilety
            List<string> tickets = new List<string>();
            foreach (var elem in db.TICKETS.ToList())
            {
                tickets.Add(elem.TYPE);
            }

            ViewBag.ticketstype = new SelectList(tickets);


            return View();
        }

        [HttpPost]
        public ActionResult SummarySeats(string[] seat)
        {
            Dictionary<string, string> seatTicket = new Dictionary<string, string>();
            string []seat_ticket;
            foreach (var item in seat)
            {
                //klucz to zajete siedzenie, wartosc rodzaj biletu
                seat_ticket = item.Split(' ');
                seatTicket.Add(seat_ticket[0], seat_ticket[1]);
            }

            ViewBag.seatTicket = seatTicket;
            //return Content(s);
            return View();
        }

        [HttpPost]
        public ActionResult SummaryReservation()
        {
            //RESERVATIONS rESERVATIONS=new RESERVATIONS();
            //rESERVATIONS.ID_PROGRAM = 2;
            //rESERVATIONS.USER_LOGIN = "Kowalski_93";
            //db.RESERVATIONS.Add(rESERVATIONS);
            //db.SaveChanges();
            return RedirectToAction("Index");
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
