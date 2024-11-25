using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    //[Authorize]
    public class UsersController : Controller
    {
        private MyStartDBEntities db = new MyStartDBEntities();

        // GET: Users

        public ActionResult Index()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
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


                // Check if email already exists
                if (db.Users.Any(x => x.Email == user.Email))
                {
                    Response.Write("<script>alert('Email Already Registered')</script>");
                    return View(user);
                }

                // Check if username already exists
                if (db.Users.Any(x => x.Username == user.Username))
                {
                    Response.Write("<script>alert('Username already exists')</script>");
                    return View(user);
                }

                // Check for empty fields
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    Response.Write("<script>alert('Empty field/s')</script>");
                    return View(user);
                }

                // Check if passwords match
                if (user.PasswordHash == user.ConfirmPassword)
                {

                    //#region Password Hashing
                    //user.PasswordHash = Crypto.Hash(user.PasswordHash);
                    //user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                    //#endregion

                    //Add User
                    db.Users.Add(user);
                    db.SaveChanges();

                    Response.Write("<script>alert('Registration Successful')</script>");

                    //MAC ADDRESS
                    var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                   where nic.OperationalStatus == OperationalStatus.Up
                                   select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";


                    //Audit Logs
                    //Start Audit
                    var user_id = user.UserId;
                    var audit = new MOVEHIST
                    {
                        Id = user_id,
                        OLD_DATA = "Old data placeholder",
                        NEW_DATA = $"Username={user.Username}, Email={user.Email}",
                        D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                        T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                        DESCRIPTION = "User creation",
                        ACTION_BY = "System",
                        MAC_ADDRESS = macAddr,
                        TYPE = "Account Creation",
                        NEW_SAL = "0",
                        OLD_SAL = "0"
                    };

                    // Add and save the audit record
                    db.MOVEHISTs.Add(audit);
                    db.SaveChanges();
                    //End Audit

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

        //POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.SingleOrDefault(x => x.UserId == user.UserId);
                //var existingUser = db.Users.Find(user.UserId);


                if (existingUser == null)
                {
                    return HttpNotFound("The User record does not exist");
                }

                // Capture old data for audit
                var oldData = $"Username={existingUser.Username}, Email={existingUser.Email}";

                // Update user details
                existingUser.Username = user.Username;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.Email = user.Email;
                existingUser.IsActive = user.IsActive;

                try
                {
                    db.SaveChanges();

                    // Capture MAC address
                    var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                   where nic.OperationalStatus == OperationalStatus.Up
                                   select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";

                    // Create audit log
                    var user_id = existingUser.UserId;

                    var audit = new MOVEHIST
                    {
                        Id = user_id,
                        OLD_DATA = oldData,
                        NEW_DATA = $"Username={user.Username}, Email={user.Email}",
                        D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                        T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                        DESCRIPTION = "User Edited",
                        ACTION_BY = Session["Username"]?.ToString() ?? "Unknown",
                        MAC_ADDRESS = macAddr,
                        TYPE = "Edit Account",
                        NEW_SAL = "0",
                        OLD_SAL = "0"
                    };

                    // Save audit log
                    db.MOVEHISTs.Add(audit);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "A concurrency error occurred while saving changes. Please try again.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
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

            //MAC ADDRESS
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";

            var user_id = user.UserId;
            var audit = new MOVEHIST
            {
                Id = user_id,
                OLD_DATA = "Old data placeholder",
                NEW_DATA = $"Username={user.Username}, Email={user.Email}",
                D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                DESCRIPTION = "Deleted Account",
                ACTION_BY = Session["Username"]?.ToString() ?? "Unknown",
                MAC_ADDRESS = macAddr,
                TYPE = "Account Delete",
                NEW_SAL = "0",
                OLD_SAL = "0"
            };

            // Add and save the audit record
            db.MOVEHISTs.Add(audit);
            db.SaveChanges();
            //End Audit


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
                Session["UserId"] = query.UserId.ToString();
                Session["Username"] = query.Username.ToString();
                Session["Email"] = query.Email.ToString();

                var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                               where nic.OperationalStatus == OperationalStatus.Up
                               select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";


                //Audit Logs
                //Start Audit
                var user_id = query.UserId;
                var audit = new MOVEHIST
                {
                    Id = user_id,
                    OLD_DATA = "Old data placeholder",
                    NEW_DATA = $"Username={user.Username}, Email={user.Email}",
                    D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                    T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                    DESCRIPTION = "User Logged In",
                    ACTION_BY = Session["Username"]?.ToString() ?? "Unknown",
                    MAC_ADDRESS = macAddr,
                    TYPE = "Account Login",
                    NEW_SAL = "0",
                    OLD_SAL = "0"
                };

                // Add and save the audit record
                db.MOVEHISTs.Add(audit);
                db.SaveChanges();
                //End Audit

                Response.Write("<script>alert('Login Successful')</script>");
                return RedirectToAction("Index", "Users");
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
                message = "Password reset link sent successfully.";
                var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                               where nic.OperationalStatus == OperationalStatus.Up
                               select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";

                //Audit Logs

                //Start Audit

                var user_id = account.UserId;
                var audit = new MOVEHIST
                {
                    Id = user_id,
                    OLD_DATA = "Old data placeholder",
                    NEW_DATA = $"Username={account.Username}, Email={account.Email}",
                    D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                    T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                    DESCRIPTION = "User Change Password",
                    ACTION_BY = Session["Username"]?.ToString() ?? "Unknown",
                    MAC_ADDRESS = macAddr,
                    TYPE = "Account Forgot Password",
                    NEW_SAL = "0",
                    OLD_SAL = "0"
                };

                // Add and save the audit record

                db.MOVEHISTs.Add(audit);
                db.SaveChanges();

                //End Audit

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
            Session.Clear();
            Session.Abandon();

            // Sign out of forms authentication
            System.Web.Security.FormsAuthentication.SignOut();

            // Redirect to the login page or any other page
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

