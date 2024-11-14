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
    public class UsersController : Controller
    {
        private MyStartDBEntities db = new MyStartDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive")] User user)
        {

            
            if (ModelState.IsValid)
            {
                // Check if email is existing 
                if (db.Users.Any(x => x.Email == user.Email))
                {
                    ViewBag.Message = "Email Already Registered";
                    return View(user);
                }
                // Check if username is existing
                if (db.Users.Any(x => x.Username == user.Username))
                {
                    ViewBag.Message = "Username Has Been Taken";
                    return View(user);
                }
                // Check for Empty fields
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    ViewBag.Message = "You Need To Type Something";
                    return View(user);
                }
                
                //Create An Account
                else
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    Response.Write("<Script>alert('Registration Successful')</Script>");
                    return RedirectToAction("Login");
                }
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(MyLogin user)
        {

            var query = db.Users.SingleOrDefault(x => x.Username == user.Username && x.PasswordHash == user.Password);

            if (query != null)
            {
                Response.Write("<Script>alert('Login Successful')</Script>");
                return RedirectToAction("Index");
            }
            else
            {
                Response.Write("<Script>alert('Invalid Account')</Script>");

            }

            
            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword(int? id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(MyPassword user)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email in the Users table
                var query = db.Users.FirstOrDefault(x => x.Email == user.Email);
                if (query != null)
                {
                    // Check if the new password matches the confirmation
                    if (user.PasswordHash == user.ConfirmPassword)
                    {
                        // Update the password hash 
                        query.PasswordHash = user.PasswordHash;
                        db.Entry(query).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Response.Write("<script>alert('Password reset successfully');</script>");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        Response.Write("<script>alert('Password does not match');</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('Account Not Found');</script>");
                }
            }
            else
            {
                ViewBag.Message = "You Need To Type Something";
            }

            return View(user);
        }

    }
}
