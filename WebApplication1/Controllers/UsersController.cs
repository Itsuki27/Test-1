﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    //[Authorize]
    public class UsersController : Controller
    {
        private MyStartDBEntities db = new MyStartDBEntities();

        // GET: Users
        //[AllowAnonymous]
        public ActionResult Index()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            return View(db.Users.ToList());
        }

      

        // GET: Users/Details/5
        //[AllowAnonymous]
        //[Authorize]
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
        //[AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public ActionResult Create([Bind(Include = "UserId,Username,PasswordHash,ConfirmPassword,Email,CreatedDate,IsActive,ActivationCode")] User user)
        {
            if (ModelState.IsValid)
            {
               

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

                    #region Password Hashing
                        //user.PasswordHash= Crypto.Hash(user.PasswordHash);
                        //user.ConfirmPassword= Crypto.Hash(user.ConfirmPassword);
                    #endregion


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
        //[AllowAnonymous]
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
        //[AllowAnonymous]
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
        //[AllowAnonymous]
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
        //[AllowAnonymous]
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

        //GET: Users/Login
       [HttpGet]
       //[AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //POST: Users/Login
       [HttpPost]
        //[AllowAnonymous]
        public ActionResult Login(MyLogin user)
        {
            // Check if the connection is secure (HTTPS)
            if (!Request.IsSecureConnection)
            {
                Response.Write("<script>alert('Login failed: Connection is not secure. Please use HTTPS.')</script>");
                return View();
            }

            // Validate the connection string
            if (!IsConnectionStringValid(db.Database.Connection.ConnectionString))
            {
                Response.Write("<script>alert('Login failed: Invalid connection string.')</script>");
                return View();
            }

            // Attempt to open the database connection
            using (var testConnection = db.Database.Connection)
            {
                try
                {
                    testConnection.Open(); // Open the connection
                }
                catch
                {
                    Response.Write("<script>alert('Login failed: Unable to connect to the database.')</script>");
                    return View();
                }
            }

            // Verify user credentials in the database
            var query = db.Users.SingleOrDefault(x =>
                x.Username == user.Username && x.PasswordHash == user.Password);

            if (query != null)
            {
                // Successful login
                Session["UserId"] = query.UserId.ToString();
                Session["Username"] = query.Username.ToString();
                Response.Write("<script>alert('Login Successful')</script>");
                return RedirectToAction("Index", "Users");
            }
            else
            {
                // Invalid username or password
                Response.Write("<script>alert('Invalid Username or Password')</script>");
            }

            return View();
        }

        private bool IsConnectionStringValid(string connectionString)
        {
            // Define the required substrings for validation
            string requiredDataSource = @"data source=AINO\MSSQLSERVER2024";
            string requiredInitialCatalog = @"initial catalog=MyStartDB";

            // Check if the connection string contains the required values
            return connectionString.ToLower().Contains(requiredDataSource.ToLower()) &&
                   connectionString.ToLower().Contains(requiredInitialCatalog.ToLower());
        }



        //public ActionResult Login(MyLogin user)
        //{
        //    // Define the required secure connection string pattern
        //    string secureConnectionStringPattern = @"data source=AINO\\MSSQLSERVER2024;initial catalog=MyStartDB";

        //    // Validate the connection string against the secure pattern
        //    if (!db.Database.Connection.ConnectionString.StartsWith(secureConnectionStringPattern, StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Log the issue for debugging purposes (optional)
        //        System.Diagnostics.Debug.WriteLine("Invalid or insecure connection string detected.");

        //        // Immediately log out (clear session and redirect)
        //        Session.Clear();
        //        Session.Abandon();
        //        Response.Write("<script>alert('Security policy violation: Invalid connection string. You have been logged out.')</script>");
        //        return RedirectToAction("Login", "Users");
        //    }

        //    // Proceed with login logic if the connection string is secure
        //    string hashedPassword = Crypto.Hash(user.Password);

        //    // Query for user in the database
        //    var query = db.Users.SingleOrDefault(x =>
        //        x.Username == user.Username && x.PasswordHash == hashedPassword);

        //    if (query != null)
        //    {
        //        // Successful login
        //        Session["UserId"] = query.UserId.ToString();
        //        Session["Username"] = query.Username.ToString();
        //        Response.Write("<script>alert('Login Successful')</script>");
        //        return RedirectToAction("Index", "Users");
        //    }
        //    else
        //    {
        //        // Failed login
        //        Response.Write("<script>alert('Invalid Username or Password')</script>");
        //    }

        //    return View();
        //}



        // GET: Users/ForgotPassword

        //[AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Users/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AllowAnonymous]
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
        //[AllowAnonymous]
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
        //[AllowAnonymous]
        public ActionResult ResetPassword(ResetPassword model)
        {
            string message = "";

            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(a => a.ResetPasswordCode == model.ResetCode);

                if (user != null)
                {
                    //user.PasswordHash = Crypto.Hash(model.PasswordHash);
                    user.PasswordHash = model.PasswordHash;
                    user.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    Response.Write("<script>alert('New password updated successfully')</script>");
                    //message = "New password updated successfully.";
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

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();

            //FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Users");
        }





        public void SendVeficationLink(string Email, string ActivationCode, string emailFor = "VerifyAccount")
        {
            string verifyURL = "/Users/" + emailFor + "/" + ActivationCode;
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