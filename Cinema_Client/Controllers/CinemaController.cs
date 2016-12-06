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
    public class CinemaController : Controller
    {
        private CinemaEntities db = new CinemaEntities();

        // GET: Cinema
        public ActionResult Index()
        {
            return View(db.MOVIES.ToList());
        }

        // GET: Cinema/Details/5
        public ActionResult Details(string id)
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

        // GET: Cinema/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cinema/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_MOVIE,TITLE,GENRE,RUNTIME,RATING,RELEASE_DATE,DIRECTION,SCREENPLAY,STARRING,IMAGE,TRAILER,SYNOPSIS")] MOVIES mOVIES)
        {
            if (ModelState.IsValid)
            {
                db.MOVIES.Add(mOVIES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mOVIES);
        }

        // GET: Cinema/Edit/5
        public ActionResult Edit(string id)
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

        // POST: Cinema/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_MOVIE,TITLE,GENRE,RUNTIME,RATING,RELEASE_DATE,DIRECTION,SCREENPLAY,STARRING,IMAGE,TRAILER,SYNOPSIS")] MOVIES mOVIES)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mOVIES).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mOVIES);
        }

        // GET: Cinema/Delete/5
        public ActionResult Delete(string id)
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

        // POST: Cinema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MOVIES mOVIES = db.MOVIES.Find(id);
            db.MOVIES.Remove(mOVIES);
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
