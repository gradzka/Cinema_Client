﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema_Client;
using System.Web.Caching;

namespace Cinema_Client.Controllers
{
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
                var rESERVATIONS = db.RESERVATIONS.Include(r => r.PROGRAM).Include(r => r.USERS).Include(r => r.PROGRAM.MOVIES).Where(r => r.USER_LOGIN == login).OrderBy(x => x.PROGRAM.DATE).ThenBy(x => x.PROGRAM.TIME);
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

                Dictionary<string, TICKETS> seat_ticket = new Dictionary<string, TICKETS>();

                foreach (var item in RDetails)
                {
                    seat_ticket.Add(item.ID_SEAT, item.TICKETS);
                }

                ViewBag.RDetails = seat_ticket;

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
            if (Session["USER_LOGIN"] != null)
            {
                if (movieId != null)
                {
                    List<string> dataArray = new List<string>();
                    foreach (var elem in db.PROGRAM.Where(x => x.ID_MOVIE == movieId).OrderBy(x =>x.DATE).ThenBy(x=>x.TIME).ToList())
                    {
                        dataArray.Add(elem.DATE.ToShortDateString() + " " + elem.TIME.ToString() + " Sala: " + elem.ID_HALL.ToString());
                    }

                    ViewBag.DateTimeHall = new SelectList(dataArray);

                    var MovieTitle = db.MOVIES.Where(x => x.ID_MOVIE == movieId).Select(x => x.TITLE).FirstOrDefault();

                    Session["mTitle"] = MovieTitle;

                    //do stworzenia sali kinowej

                }
                if (programId != null)
                {
                    //ViewBag.programId = programId;
                    var prId = Int32.Parse(programId);

                    var MovieIdentity = db.PROGRAM.Where(x => x.ID_PROGRAM == prId).Select(x => x.ID_MOVIE).FirstOrDefault();
                    var MoId = MovieIdentity.ToString();

                    List<string> dataArray = new List<string>();
                    foreach (var elem in db.PROGRAM.Where(x => x.ID_MOVIE == MoId).ToList())
                    {
                        dataArray.Add(elem.DATE.ToShortDateString() + " " + elem.TIME.ToString() + " Sala: " + elem.ID_HALL.ToString());
                    }

                    ViewBag.DateTime = new SelectList(dataArray);

                    var MovieTitle = db.MOVIES.Where(x => x.ID_MOVIE == MoId).Select(x => x.TITLE).FirstOrDefault();

                    Session["mTitle"] = MovieTitle;
                }
                return View();
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Next(string DateTimeHall,short? idres, Dictionary<string, string> seatTicket)
        {
            if (Session["USER_LOGIN"] != null)
            {
                //Session["date_time_hall"] = DateTimeHall;
                Session["DateTimeHall"] = DateTimeHall;

                string[] date_time_hall_Array = DateTimeHall.Split(' ');//3 -hall

                List<string> seats = new List<string>();
                var hall = date_time_hall_Array[3];
                seats = db.SEATS.Where(x => x.ID_HALL == hall).Select(x => x.ID_SEAT).ToList();

                var vip = db.SEATS.Where(x => x.ID_HALL == hall && x.VIP == true).Select(x => x.ID_SEAT).ToList();
                ViewBag.vip = vip;

                //liczba rzedow - wymiar pionowy
                //liczba siedzien - wymiar poziomy

                int seats_number = seats.Count();
                string last_letter = seats.Last();
                int rows_number = (int)(last_letter[0]) - 65 + 1;
                int columns_number = seats_number / rows_number;

                //Session["rows_nO"]
                ViewBag.rows_No = rows_number;
                ViewBag.columns_No = columns_number;
                ViewBag.screen = columns_number - 4;
                ViewBag.lastletter = last_letter[0];

                
                /*
                 * miejsca zajete: PROGRAM(ID_PROGRAM, ID_HALL)
                 * RESERVATIONS(ID_PROGRAM, ID_RESERVATION)
                 * RESERVATIONS_DETAILS(ID_SEAT, ID_RESERVATION)
                 */
                string[] date = date_time_hall_Array[0].Split('-');
                //year, month, day
                DateTime YearMonthDay = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]));
                //hour, minutes, seconds
                string[] time = date_time_hall_Array[1].Split(':');
                TimeSpan HourMinutesSeconds = new TimeSpan(Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));

                string Hallid = date_time_hall_Array[3];
                //ID_Program
                var idProgram = db.PROGRAM.Where
                    (x => x.DATE == YearMonthDay &&
                    x.TIME == HourMinutesSeconds &&
                    x.ID_HALL == Hallid).Select(x => x.ID_PROGRAM).FirstOrDefault();

                var idMovie = db.PROGRAM.Where(x => x.ID_PROGRAM == idProgram).Select(x => x.ID_MOVIE).FirstOrDefault();

                if (Session["mTitle"] == null)
                {              
                    Session["mTitle"] = db.MOVIES.Where(x => x.ID_MOVIE == idMovie).Select(x => x.TITLE).FirstOrDefault();
                }

                byte movieRating = db.MOVIES.Where(x => x.ID_MOVIE == idMovie).Select(x => x.RATING).FirstOrDefault();
                //Bilety
                List<string> tickets = new List<string>();
                foreach (var elem in db.TICKETS.ToList())
                {
                    if (movieRating > 7 && elem.TYPE == "Dziecko") continue;
                    tickets.Add(elem.TYPE);
                }

                ViewBag.ticketstype = new SelectList(tickets);

                //lista zajetych krzesel
                var bookedSeat = db.RESERVATIONS_DETAILS.Include(r => r.RESERVATIONS).Include(r => r.RESERVATIONS.PROGRAM).Where(r => r.RESERVATIONS.ID_PROGRAM == idProgram).Select(r => r.ID_SEAT).ToList();
                

                ViewBag.idReservation = (short)-1; //uzytkownik dokonuje rezerwacji, a nie modyfikacji rezerwacji
                ViewBag.selectedSeatTicket = new Dictionary<string, string>();
                //miejsca wybrane przez uzytkownika
                if (seatTicket != null && idres> -1)
                {
                    ViewBag.idReservation = idres;
                    ViewBag.selectedSeatTicket = seatTicket;
                    foreach (var item in seatTicket)
                    {
                        bookedSeat.Remove(item.Key); //usuniecie z bookedSeat siedzen zajetych przez uzytkownika, ktore moze chciec edytowac
                    }
                    
                }
                ViewBag.bookedSeat = bookedSeat;
                return View();
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SummarySeats(string[] seat, short? id)
        {
            ViewBag.idReservation = id;
            if (Session["USER_LOGIN"] != null)
            {
                Dictionary<string, string> seatTicket = new Dictionary<string, string>();

                string[] date_time_hall_Array = Session["DateTimeHall"].ToString().Split(' ');//3 -hall
                var hall = date_time_hall_Array[3];
                var vip = db.SEATS.Where(x => x.ID_HALL == hall && x.VIP == true).Select(x => x.ID_SEAT).ToList();
                string[] seat_ticket;
                foreach (var item in seat)
                {
                    //klucz to zajete siedzenie, wartosc rodzaj biletu

                    seat_ticket = item.Split(' ');
                    if (vip.Contains(seat_ticket[0]) == true) { seat_ticket[0] += " VIP"; }
                    seatTicket.Add(seat_ticket[0], seat_ticket[1]);
                }

                //ViewBag.seatTicket = seatTicket;
                Session["seatTicket"] = seatTicket;
                //return Content(s);
                return View();
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        [HttpPost]
        public ActionResult SummaryReservation(short? id)
        {
            if (Session["USER_LOGIN"] != null)
            {
                if (id == -1) //uzytkownik dokonuje nowej rezerwacji
                {
                    RESERVATIONS rESERVATIONS = new RESERVATIONS();
                    string dateTimeHall = Session["DateTimeHall"].ToString();
                    string[] date_time_hall_Array = dateTimeHall.Split(' ');//3 -hall
                    string[] date = date_time_hall_Array[0].Split('-');
                    //year, month, day
                    DateTime YearMonthDay = new DateTime(Int32.Parse(date[0]), Int32.Parse(date[1]), Int32.Parse(date[2]));
                    //hour, minutes, seconds
                    string[] time = date_time_hall_Array[1].Split(':');
                    TimeSpan HourMinutesSeconds = new TimeSpan(Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));

                    string Hallid = date_time_hall_Array[3];
                    //ID_Program
                    var idProgram = db.PROGRAM.Where
                        (x => x.DATE == YearMonthDay &&
                        x.TIME == HourMinutesSeconds &&
                        x.ID_HALL == Hallid).Select(x => x.ID_PROGRAM).FirstOrDefault();

                    //rESERVATIONS.ID_RESERVATION=

                    string userLogin = Session["USER_LOGIN"].ToString();


                    rESERVATIONS.ID_PROGRAM = idProgram;
                    rESERVATIONS.USER_LOGIN = userLogin;
                    db.RESERVATIONS.Add(rESERVATIONS);
                    db.SaveChanges();

                    //id_reser  |   id_seat     |id_ticket
                    RESERVATIONS_DETAILS rESERVATIONS_DETAILS;

                    var idReservation = rESERVATIONS.ID_RESERVATION;
                    Dictionary<string, string> seatTicket = (Dictionary<string, string>)Session["seatTicket"];

                    //pair.Value - rodzaj biletu
                    //pair.Key - miejsce
                    foreach (KeyValuePair<string, string> pair in seatTicket)
                    {
                        rESERVATIONS_DETAILS = new RESERVATIONS_DETAILS();
                        rESERVATIONS_DETAILS.ID_RESERVATION = idReservation;
                        if (pair.Key.Contains("VIP"))
                        { rESERVATIONS_DETAILS.ID_SEAT = pair.Key.Remove(pair.Key.Length - 4); }
                        else { rESERVATIONS_DETAILS.ID_SEAT = pair.Key; }
                        rESERVATIONS_DETAILS.ID_TICKET = db.TICKETS.Where(x => x.TYPE == pair.Value).Select(x => x.ID_TICKET).FirstOrDefault();
                        db.RESERVATIONS_DETAILS.Add(rESERVATIONS_DETAILS);
                    }
                }
                else //uzytkownik ma rezerwacje
                {
                    List <RESERVATIONS_DETAILS> rESERVATIONS_DETAILS_List = db.RESERVATIONS_DETAILS.Where(x=> x.ID_RESERVATION==id).ToList(); //zwraca wszystkie rezerwacje o ID==id uzytkownika
                    Dictionary<string, string> seatTicket = (Dictionary<string, string>)Session["seatTicket"]; //siedzenia i bilety, ktore wybrał uzytkownik

                    //1. Skasowanie pozycji z RESERVATIONS_DETAILS o id przeslanym w argumencie metody
                    foreach (var item in rESERVATIONS_DETAILS_List)
                    {
                        db.RESERVATIONS_DETAILS.Remove(item);
                    }

                    db.SaveChanges();
                    RESERVATIONS_DETAILS rESERVATIONS_DETAILS;

                    //2. Dodanie do bazy poprawnych pozycji z listy
                    foreach (KeyValuePair<string, string> pair in seatTicket)
                    {
                        rESERVATIONS_DETAILS = new RESERVATIONS_DETAILS();
                        rESERVATIONS_DETAILS.ID_RESERVATION = id.Value;
                        if (pair.Key.Contains("VIP"))
                        { rESERVATIONS_DETAILS.ID_SEAT = pair.Key.Remove(pair.Key.Length - 4); }
                        else { rESERVATIONS_DETAILS.ID_SEAT = pair.Key; }
                        rESERVATIONS_DETAILS.ID_TICKET = db.TICKETS.Where(x => x.TYPE == pair.Value).Select(x => x.ID_TICKET).FirstOrDefault();
                        db.RESERVATIONS_DETAILS.Add(rESERVATIONS_DETAILS);
                    }

                }
                Session["mTitle"] = null;
                Session["DateTimeHall"] = null;
                Session["seatTicket"] = null;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Reservation/Delete/5
        public ActionResult Delete(short? id)
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
                Dictionary<string, string> seatTicket = new Dictionary<string, string>();

                foreach (var item in RDetails)
                {
                    seatTicket.Add(item.ID_SEAT, db.TICKETS.Where(x => x.ID_TICKET == item.ID_TICKET).Select(x => x.TYPE).FirstOrDefault());
                }

                Session["seatTicket"] = seatTicket;

                return View(rESERVATIONS);
            }
            else
            {
                return RedirectToAction("", "Home");
            }
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            if (Session["USER_LOGIN"] != null)
            {
                RESERVATIONS rESERVATIONS = db.RESERVATIONS.Find(id);
                db.RESERVATIONS.Remove(rESERVATIONS);
                db.SaveChanges();
            }
            Session["seatTicket"] = null;
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
