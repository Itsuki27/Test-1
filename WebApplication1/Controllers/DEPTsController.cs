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
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            return View(dEPT);
        }

        // GET: DEPTs/Create
        public ActionResult Create()
        {

            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            return View();
        }

        // POST: DEPTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DEPT_ID,DEPT1,DEPT_DESC,DEPT_BLDG,DEPT_FLOOR")] DEPT dEPT)
        {

            bool Duplicate = db.DEPTS.Any(x => x.DEPT1 == dEPT.DEPT1 && x.DEPT_ID != dEPT.DEPT_ID);

            if (Duplicate == true)
            {
             
                ModelState.AddModelError("DEPT1", "Department already exists");
                return View(dEPT);
            }

            if (ModelState.IsValid)
            {
                db.DEPTS.Add(dEPT);
                db.SaveChanges();

                TempData["DeptCreate"] = "<script>Swal.fire({icon: 'success', title: 'Department Creation Success!'});</script>";
                return RedirectToAction("Index");
            }

            return View(dEPT);
        }

        // GET: DEPTs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Warning"] = null;
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == 1)
            {
                TempData["Warning"] = "<script>Swal.fire({icon: 'error', title: 'Action Not Allowed!'});</script>";
                return RedirectToAction("Index");
            }

            DEPT dEPT = db.DEPTS.Find(id);
            if (dEPT == null)
            {
                return HttpNotFound();
            }

            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            TempData["Warning"] = null;
            return View(dEPT);
        }

        // POST: DEPTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DEPT_ID,DEPT1,DEPT_DESC,DEPT_BLDG,DEPT_FLOOR")] DEPT dEPT)
        {

            bool Duplicate = db.DEPTS.Any(x => x.DEPT1 == dEPT.DEPT1 && x.DEPT_ID != dEPT.DEPT_ID);

            if (Duplicate == true)
            {
                ModelState.AddModelError("DEPT1", "Department already exists");
                return View(dEPT);
            }

            if (ModelState.IsValid)
            {
                db.Entry(dEPT).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["DeptEdit"] = "<script>Swal.fire({icon: 'success', title: 'Department Updated!'});</script>";
                return RedirectToAction("Index", "DEPTs");
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
            if (id == 1)
            {
                TempData["Warning"] = "<script>Swal.fire({icon: 'error', title: 'Action Not Allowed!'});</script>";
                return RedirectToAction("Index");
            }
            DEPT dEPT = db.DEPTS.Find(id);
            if (dEPT == null)
            {
                return HttpNotFound();
            }

            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
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

            TempData["DeptDelete"] = "<script>Swal.fire({icon: 'success', title: 'Department Deleted!'});</script>";

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


