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
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
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

    }
}


