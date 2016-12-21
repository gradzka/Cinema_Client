using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema_Client.Models;

namespace Cinema_Client.Controllers
{
    public class ProgramController : Controller
    {
        private CinemaEntities db = new CinemaEntities();

        // GET: Program
        public ActionResult Index()
        {
            var pROGRAM = db.PROGRAM.Include(p => p.HALLS).Include(p => p.MOVIES);
            return View(pROGRAM.ToList());
        }

        // GET: Program/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROGRAM pROGRAM = db.PROGRAM.Find(id);
            if (pROGRAM == null)
            {
                return HttpNotFound();
            }
            return View(pROGRAM);
        }

        // GET: Program/Create
        public ActionResult Create()
        {
            ViewBag.ID_HALL = new SelectList(db.HALLS, "ID_HALL", "ID_HALL");
            ViewBag.ID_MOVIE = new SelectList(db.MOVIES, "ID_MOVIE", "TITLE");
            return View();
        }

        // POST: Program/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_PROGRAM,DATE,TIME,ID_HALL,ID_MOVIE,C2D_3D,VERSION")] PROGRAM pROGRAM)
        {
            if (ModelState.IsValid)
            {
                db.PROGRAM.Add(pROGRAM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_HALL = new SelectList(db.HALLS, "ID_HALL", "ID_HALL", pROGRAM.ID_HALL);
            ViewBag.ID_MOVIE = new SelectList(db.MOVIES, "ID_MOVIE", "TITLE", pROGRAM.ID_MOVIE);
            return View(pROGRAM);
        }

        // GET: Program/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROGRAM pROGRAM = db.PROGRAM.Find(id);
            if (pROGRAM == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_HALL = new SelectList(db.HALLS, "ID_HALL", "ID_HALL", pROGRAM.ID_HALL);
            ViewBag.ID_MOVIE = new SelectList(db.MOVIES, "ID_MOVIE", "TITLE", pROGRAM.ID_MOVIE);
            return View(pROGRAM);
        }

        // POST: Program/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_PROGRAM,DATE,TIME,ID_HALL,ID_MOVIE,C2D_3D,VERSION")] PROGRAM pROGRAM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pROGRAM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_HALL = new SelectList(db.HALLS, "ID_HALL", "ID_HALL", pROGRAM.ID_HALL);
            ViewBag.ID_MOVIE = new SelectList(db.MOVIES, "ID_MOVIE", "TITLE", pROGRAM.ID_MOVIE);
            return View(pROGRAM);
        }

        // GET: Program/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROGRAM pROGRAM = db.PROGRAM.Find(id);
            if (pROGRAM == null)
            {
                return HttpNotFound();
            }
            return View(pROGRAM);
        }

        // POST: Program/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            PROGRAM pROGRAM = db.PROGRAM.Find(id);
            db.PROGRAM.Remove(pROGRAM);
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
