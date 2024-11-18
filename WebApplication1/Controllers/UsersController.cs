using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Username,PasswordHash,ConfirmPassword,Email,CreatedDate,IsActive,ActivationCode")] User user)
        {
            if (ModelState.IsValid)
            {
                #region Generate Activation Code
                //user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                user.PasswordHash = Crypto.Hash(user.PasswordHash);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                // Check if email already exists
                if (db.Users.Any(x => x.Email == user.Email))
                {
                    Response.Write("<script>alert('Email Already Registered')</script>");
                    return View(user);
                }

                // Check if username already exists
                if (db.Users.Any(x => x.Username == user.Username))
                {
                    Response.Write("<script>alert('Username Has Been Taken')</script>");
                    return View(user);
                }

                // Check for empty fields
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    Response.Write("<script>alert('You Need To Type Something')</script>");
                    return View(user);
                }

                // Check if passwords match
                if (user.PasswordHash == user.ConfirmPassword)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    Response.Write("<script>alert('Registration Successful')</script>");
                    return RedirectToAction("Login");
                }
                else
                {
                    Response.Write("<script>alert('Password Does Not Match')</script>");
                    return View(user);
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

        // GET: Users/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        public ActionResult Login(MyLogin user)
        {
            var query = db.Users.SingleOrDefault(x => x.Username == user.Username && x.PasswordHash == user.Password);

            if (query != null)
            {
                Response.Write("<script>alert('Login Successful')</script>");
                return RedirectToAction("Index");
            }
            else
            {
                Response.Write("<script>alert('Invalid Account')</script>");
            }

            return View();
        }

        // GET: Users/ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Users/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {
            string message = "";
            var account = db.Users.FirstOrDefault(a => a.Email == Email);

            if (account != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVeficationLink(account.Email, resetCode, "ResetPassword");
                account.ResetPasswordCode = resetCode;

                db.SaveChanges();
                message = "Reset password link sent successfully.";
            }
            else
            {
                message = "Account not found.";
            }

            ViewBag.Message = message;
            return View();
        }

        // GET: Users/ResetPassword
        public ActionResult ResetPassword(string id)
        {
            var query = db.Users.FirstOrDefault(x => x.ResetPasswordCode == id);

            if (query != null)
            {
                ResetPassword model = new ResetPassword { ResetCode = id };
                return View(model);
            }

            return HttpNotFound();
        }

        // POST: Users/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            string message = "";

            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(a => a.ResetPasswordCode == model.ResetCode);

                if (user != null)
                {
                    user.PasswordHash = Crypto.Hash(model.PasswordHash);
                    user.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "New password updated successfully.";
                }
                else
                {
                    message = "Invalid reset link.";
                }
            }
            else
            {
                message = "Invalid data.";
            }

            ViewBag.Message = message;
            return View(model);
        }

        public void SendVeficationLink(string Email, string ActivationCode, string emailFor = "VerifyAccount")
        {
            string verifyURL = "/User/" + emailFor + "/" + ActivationCode;
            string link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyURL);

            var fromEmail = new MailAddress("pobletemar6@gmail.com", "EmailSender");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "rgyn spwl vops yyoo";

            string subject = emailFor == "VerifyAccount" ? "Your Account is Successfully Created" : "Reset Password";
            string body = emailFor == "VerifyAccount"
                ? $"<br/><br/> PH NET<br/>Please click on the link below to verify your account:<br/><a href='{link}'>{link}</a>"
                : $"Hi,<br/><br/>We received a request to reset your password. Please click the link below:<br/><a href='{link}'>Reset Password Link</a>";

            using (var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
                EnableSsl = true
            })
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
