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
    public class DEPTsController : Controller
    {
        private MyStartDBEntities db = new MyStartDBEntities();

        // GET: DEPTs
        public ActionResult Index()
        {
            return View(db.DEPTS.ToList());
        }

        // GET: DEPTs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPT dEPT = db.DEPTS.Find(id);
            if (dEPT == null)
            {
                return HttpNotFound();
            }
            return View(dEPT);
        }

        // GET: DEPTs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DEPTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DEPT_ID,DEPT1,DEPT_DESC,DEPT_BLDG,DEPT_FLOOR")] DEPT dEPT, User user)
        {
            if (ModelState.IsValid)
            {

                db.DEPTS.Add(dEPT);
                db.SaveChanges();


                //var id = user.UserId;
                //var department = new User
                //{
                //    DEPT_ID = dEPT.DEPT_ID
                //}


               
                return RedirectToAction("Index");
            }

            return View(dEPT);
        }


        //var audit_id = user.UserId;
        //var audit = new MOVEHIST
        //{
        //    Id = audit_id,
        //    OLD_DATA = $"Username={user.Username}, Email={user.Email}",
        //    NEW_DATA = $"Username={user.Username}, Email={user.Email}",
        //    D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
        //    T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
        //    DESCRIPTION = "User creation",
        //    ACTION_BY = "System",
        //    MAC_ADDRESS = macAddr,
        //    TYPE = "Account Creation",
        //    NEW_SAL = "0",
        //    OLD_SAL = "0"
        //};

        //// Add and save the audit record
        //db.MOVEHISTs.Add(audit);
        //            db.SaveChanges();
        //            //End Audit

        // GET: DEPTs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPT dEPT = db.DEPTS.Find(id);
            if (dEPT == null)
            {
                return HttpNotFound();
            }
            return View(dEPT);
        }

        // POST: DEPTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DEPT_ID,DEPT1,DEPT_DESC,DEPT_BLDG,DEPT_FLOOR")] DEPT dEPT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dEPT).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dEPT);
        }

        // GET: DEPTs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPT dEPT = db.DEPTS.Find(id);
            if (dEPT == null)
            {
                return HttpNotFound();
            }
            return View(dEPT);
        }

        // POST: DEPTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DEPT dEPT = db.DEPTS.Find(id);
            db.DEPTS.Remove(dEPT);
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
