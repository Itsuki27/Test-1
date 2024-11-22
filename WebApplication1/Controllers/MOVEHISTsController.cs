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
        private MyStartDBEntities db = new MyStartDBEntities();

        // GET: MOVEHISTs
        public ActionResult AuditLog()
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
        public ActionResult Create([Bind(Include = "MOVEHIST_ID,MAC_ADDRESS,TYPE,OLD_DATA,NEW_DATA,OLD_SAL,NEW_SAL,D_ACTION,T_ACTION,DESCRIPTION,ACTION_BY,Id")] MOVEHIST mOVEHIST)
        {
            if (ModelState.IsValid)
            {
                db.MOVEHISTs.Add(mOVEHIST);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mOVEHIST);
        }

    }
}
