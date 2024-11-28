using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
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

            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
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

            try
            {
                using (var connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString))
                {
                    connection.Open();


                    if (connection.State == ConnectionState.Open)
                    {
                        // Success (connection opened)
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Connection Error! Invalid Database";
                        return View();
                    }
                }
            }
            catch (SqlException)
            {
                Response.Write("<script>alert('Wrong Connection Error! Invalid Database')</script>");
                return View();
            }

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
                    user.PasswordHash = Hashing.Hash(user.PasswordHash);
                    user.ConfirmPassword = Hashing.Hash(user.ConfirmPassword);
                    user.ConfirmPassword = Hashing.Hash(user.ConfirmPassword);
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
                        OLD_DATA = $"Username={user.Username}, Email={user.Email}",
                        NEW_DATA = $"Username={user.Username}, Email={user.Email}",
                        D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                        T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                        DESCRIPTION = "User creation",
                        ACTION_BY = user.Username,
                        MAC_ADDRESS = macAddr,
                        TYPE = "Account Creation",
                        NEW_SAL = "0",
                        OLD_SAL = "0"
                    };

                    // Add and save the audit record
                    db.MOVEHISTs.Add(audit);
                    db.SaveChanges();
                    //End Audit

                    TempData["UserCreate"] = "<script>Swal.fire({icon: 'success', title: 'Account Creation Success!'});</script>";

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

            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            user.ConfirmPassword = user.PasswordHash;

            PopulateDropDown();

            return View(user);
        }

        //POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive,ConfirmPassword,DEPT_ID,DEPT1")] User user)
        {
            if (ModelState.IsValid)
            {
                // check if it matches an ID
                var existingUser = db.Users.SingleOrDefault(x => x.UserId == user.UserId);

                // Fetch the list of departments
                List<DEPT> deptList = db.DEPTS.ToList();
                // Map DEPT_ID to DEPT name
                ViewBag.DropUser = new SelectList(deptList, "DEPT_ID", "DEPT1");


                if (existingUser == null)
                {
                    return HttpNotFound("The User record does not exist");
                }
                // Check if the username is duplicated
                if (db.Users.Any(x => x.Username == user.Username && x.UserId != user.UserId))
                {
                    Response.Write("<script>alert('Username already exists')</script>");
                    return View(user);
                }
                // Check if the email is duplicated
                if (db.Users.Any(x => x.Email == user.Email && x.UserId != user.UserId))
                {
                    Response.Write("<script>alert('Email already exists')</script>");
                    return View(user);
                }
                // Empty Fields
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email))
                {
                    Response.Write("<script>alert('One or more fields are empty. Please fill in all required fields.')</script>");
                    return View(user);
                }

                // Check if the password is being updated
                if (user.PasswordHash != user.ConfirmPassword)
                {
                    Response.Write("<script>alert('Passwords do not match.')</script>");
                    return View(user);
                }
                else
                {

                    if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != existingUser.PasswordHash)
                    {
                        // Validate the new password
                        if (user.PasswordHash.Length < 8 || user.PasswordHash.Length > 12)
                        {
                            Response.Write("<script>alert('Password must be between 8 and 12 characters.')</script>");
                            return View(user);
                        }

                        // Update the password if valid
                        existingUser.PasswordHash = Hashing.Hash(user.PasswordHash);
                    }
                }

                // Capture old data for audit
                var oldData = $"Username={existingUser.Username}, Email={existingUser.Email}";

                // Update user details
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.IsActive = user.IsActive;
                existingUser.DEPT_ID = user.DEPT_ID;

                db.SaveChanges();

                // Audit log
                var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                               where nic.OperationalStatus == OperationalStatus.Up
                               select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";

                var audit = new MOVEHIST
                {
                    Id = existingUser.UserId,
                    OLD_DATA = oldData,
                    NEW_DATA = $"Username={user.Username}, Email={user.Email}",
                    D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                    T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                    DESCRIPTION = "User Edited",
                    ACTION_BY = Session["Username"]?.ToString() ?? "Unknown",
                    MAC_ADDRESS = macAddr,
                    TYPE = "Account Edit",
                    NEW_SAL = "0",
                    OLD_SAL = "0"
                };

                db.MOVEHISTs.Add(audit);
                db.SaveChanges();

                TempData["UserEdit"] = "<script>Swal.fire({icon: 'success', title: 'User Updated!'});</script>";

                return RedirectToAction("Index");


            }

            return View(user);

        }

        private void PopulateDropDown()
        {
            List<DEPT> deptList = db.DEPTS.ToList();
            ViewBag.DropUser = new SelectList(deptList, "DEPT_ID", "DEPT1");
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
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
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
                DESCRIPTION = "User Deleted",
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

            // Check if the deleted user is the same as the currently logged-in user
            if (Session["Username"] != null && Session["Username"].ToString() == user.Username)
            {
                // Clear session and redirect to login
                Session.Clear();
                //TempData["UserDelete"] = "<script>Swal.fire({icon: 'success', title: 'Your account has been deleted!'});</script>";
                return RedirectToAction("Login", "Users");
            }

            TempData["UserDelete"] = "<script>Swal.fire({icon: 'success', title: 'User Deleted!'});</script>";

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

        public ActionResult Login([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive,ConfirmPassword")] MyLogin user)
        {
            try
            {
                using (var connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString))
                {
                    connection.Open();


                    if (connection.State == ConnectionState.Open)
                    {
                        // Success (connection opened)
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Connection Error! Invalid Database";
                        return View();
                    }
                }
            }
            catch (SqlException)
            {
                Response.Write("<script>alert('Wrong Connection Error! Invalid Database')</script>");
                return View();
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash))
            {
                Response.Write("<script>alert('One or more fields are empty. Please fill in all required fields.')</script>");
                return View();
            }

            var hashedPassword = Hashing.Hash(user.PasswordHash);
            var query = db.Users.SingleOrDefault(x => x.Username == user.Username && x.PasswordHash == hashedPassword);

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
                    OLD_DATA = $"Username={query.Username}, Email={query.Email}",
                    NEW_DATA = $"Username={query.Username}, Email={query.Email}",
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

                TempData["UserLogin"] = "<script>Swal.fire({icon: 'success', title: 'Login Success!'});</script>";

                return RedirectToAction("Index", "Users");
            }
            else
            {
                ViewBag.LoginError = "Invalid credentials!";
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
                    OLD_DATA = $"Username={account.Username}, Email={account.Email}",
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
                    user.PasswordHash = Hashing.Hash(model.PasswordHash);
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



        public ActionResult Logout([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive,ConfirmPassword,DEPT_ID,DEPT1")] User user)
        {
            var existingUser = db.Users.SingleOrDefault(x => x.UserId == user.UserId);

            // Get the MAC address
            var macAddr = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.OperationalStatus == OperationalStatus.Up
                           select nic.GetPhysicalAddress().ToString()).FirstOrDefault() ?? "Unknown";

            // Audit Logs
            var user_id = user.UserId;
            var displayEmail = user.Email;
            var displayUsername = user.Username;

            var audit = new MOVEHIST
            {
                Id = user_id,
                OLD_DATA = $"Username={displayUsername}, Email={displayEmail}",
                NEW_DATA = $"Username={displayUsername}, Email={displayEmail}",
                D_ACTION = DateTime.Now.ToString("MM/dd/yyyy"),
                T_ACTION = DateTime.Now.ToString("HH:mm:ss"),
                DESCRIPTION = "User Has Logout",
                ACTION_BY = Session["Username"]?.ToString() ?? "Unknown",
                MAC_ADDRESS = macAddr,
                TYPE = "Account Logout",
                NEW_SAL = "0",
                OLD_SAL = "0"
            };

            // Save the audit record before clearing the session
            db.MOVEHISTs.Add(audit);
            db.SaveChanges();

            Session.Clear();
            Session.Abandon();

            // Sign out of forms authentication
            System.Web.Security.FormsAuthentication.SignOut();


            // Redirect to the login page or any other page
            return RedirectToAction("Login", "Users");
        }


        // GET: Reports

        public ActionResult Reports()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            return View(db.Users.ToList());
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


