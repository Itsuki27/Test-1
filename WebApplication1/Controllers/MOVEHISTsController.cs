using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MOVEHISTsController : Controller
    {
        private AuditLogEntities db = new AuditLogEntities();

        // GET: MOVEHISTs
        public ActionResult Index()
        {
            return View(db.MOVEHISTs.ToList());
        }

        // GET: MOVEHISTs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MOVEHIST mOVEHIST = db.MOVEHISTs.Find(id);
            if (mOVEHIST == null)
            {
                return HttpNotFound();
            }
            return View(mOVEHIST);
        }

        // GET: MOVEHISTs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MOVEHISTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAuditLogs([Bind(Include = "MOVEHIST_ID,lineno,TYPE,OLD_DATA,NEW_DATA,OLD_SAL,NEW_SAL,D_ACTION,T_ACTION,DESCRIPTION,ACTION_BY,Id")] MOVEHIST mOVEHIST)
        {
            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                db.MOVEHISTs.Add(mOVEHIST);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                message = "Invalid Request";
            }

            return View(mOVEHIST);
        }

        // GET: MOVEHISTs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MOVEHIST mOVEHIST = db.MOVEHISTs.Find(id);
            if (mOVEHIST == null)
            {
                return HttpNotFound();
            }
            return View(mOVEHIST);
        }

        // POST: MOVEHISTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MOVEHIST_ID,lineno,TYPE,OLD_DATA,NEW_DATA,OLD_SAL,NEW_SAL,D_ACTION,T_ACTION,DESCRIPTION,ACTION_BY,Id")] MOVEHIST mOVEHIST)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mOVEHIST).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mOVEHIST);
        }

        // GET: MOVEHISTs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MOVEHIST mOVEHIST = db.MOVEHISTs.Find(id);
            if (mOVEHIST == null)
            {
                return HttpNotFound();
            }
            return View(mOVEHIST);
        }

        // POST: MOVEHISTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MOVEHIST mOVEHIST = db.MOVEHISTs.Find(id);
            db.MOVEHISTs.Remove(mOVEHIST);
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
