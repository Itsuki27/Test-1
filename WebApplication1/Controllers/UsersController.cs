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
using System.Threading;
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
                return View("Error");
            }

            User user = db.Users.Find(id);
            if (user == null)
            {
                return View("Error");
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
                    ModelState.AddModelError("Email", "Email Already Registered");
                    return View(user);
                }

                // Check if username already exists
                if (db.Users.Any(x => x.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Username Already Registered");
                    return View(user);
                }

                // Check for empty fields
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                   

                    if (string.IsNullOrEmpty(user.Username))
                    {
                        ModelState.AddModelError("Username", "Username field is empty");
                    }
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        ModelState.AddModelError("Email", "Email field is empty");
                    }
                    if (string.IsNullOrEmpty(user.ConfirmPassword))
                    {
                        ModelState.AddModelError("ConfirmPassword", "Confirm Password field is empty");
                    }
                    if (string.IsNullOrEmpty(user.PasswordHash))
                    {
                        ModelState.AddModelError("PasswordHash", "Password field is empty");
                    }
                    return View();
                }

                if (user.PasswordHash.Length < 8 || user.ConfirmPassword.Length < 8)
                {
                    ModelState.AddModelError("PasswordHash", "Password must be at least 8 characters");
                    ModelState.AddModelError("ConfirmPassword", "Password must be at least 8 characters");
                    return View(user);
                }

                if (user.DEPT_ID == null)
                {
                    TempData["deptEmpty"] = true;
                }

                // Check if passwords match
                if (user.PasswordHash == user.ConfirmPassword)
                {

                    #region Password Hashing
                    user.PasswordHash = Hashing.Hash(user.PasswordHash);
                    user.ConfirmPassword = Hashing.Hash(user.ConfirmPassword);
                    #endregion

                    //Add User
                    db.Users.Add(user);
                    db.SaveChanges();

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
                    ModelState.AddModelError("ConfirmPassword", "Password does not match");
                    
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
                    ModelState.AddModelError("Username", "Username is unavailable");
                    return View(user);
                }
                // Check if the email is duplicated
                if (db.Users.Any(x => x.Email == user.Email && x.UserId != user.UserId))
                {
                    ModelState.AddModelError("Email", "Email is unavailable");
                    return View(user);
                }
                // Empty Fields
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("Username", "Username field is empty.");
                    ModelState.AddModelError("Email", "Email field is empty.");
                    return View(user);
                }

                // Check if the password is being updated
                if (user.PasswordHash != user.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password does not match");
                    return View(user);
                }
                else
                {

                    if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash != existingUser.PasswordHash)
                    {
                        // Validate the new password
                        if (user.PasswordHash.Length < 8 || user.PasswordHash.Length > 12)
                        {
                            ModelState.AddModelError("PasswordHash", "Password must be at least 8 characters");
                            ModelState.AddModelError("ConfirmPassword", "Password must be at least 8 characters");
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

                if (existingUser.DEPT_ID == null) {
                    TempData["deptEmpty"] = true;
                }

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

                TempData["UserDelete"] = "<script>Swal.fire({icon: 'success', title: 'User Deleted!'});</script>";
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

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("Email", "Email field is empty");
                }
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    ModelState.AddModelError("PasswordHash", "Password field is empty");
                }

                return View();
            }

            var hashedPassword = Hashing.Hash(user.PasswordHash);
            var query = db.Users.SingleOrDefault(x => x.Email == user.Email && x.PasswordHash == hashedPassword);

            if (query != null)
            {
                Session["UserId"] = query.UserId;
                Session["Username"] = query.Username.ToString();
                Session["Email"] = query.Email.ToString();

                TempData["user_id"] = query.UserId;

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


            var account = db.Users.FirstOrDefault(a => a.Email == Email);

            if (account != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVeficationLink(account.Email, resetCode, "ResetPassword");
                account.ResetPasswordExpiry = DateTime.Now.AddSeconds(30);
                account.ResetPasswordCode = resetCode;
                db.SaveChanges();

                TempData["UserForgot"] = "<script>Swal.fire({icon: 'success', title: 'Password Reset Link Sent!'});</script>";

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

                return RedirectToAction("Login", "Users");

            }

            else

            {
                TempData["UserForgotErr"] = "<script>Swal.fire({icon: 'error', title: 'Account not found!'});</script>";

                return View();
            }

        }


        // GET: Users/ResetPassword

        public ActionResult ResetPassword(string id)
        {
            var query = db.Users.FirstOrDefault(x => x.ResetPasswordCode == id);

            if (query != null)
            {
                if (query.ResetPasswordExpiry.HasValue && query.ResetPasswordExpiry.Value > DateTime.Now)
                {
                    ResetPassword model = new ResetPassword { ResetCode = id };
                    return View(model);
                }
                else
                {
                    return View("LinkExpired");
                }
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
                    if (user.ResetPasswordExpiry.HasValue && user.ResetPasswordExpiry.Value > DateTime.Now)
                    {
                        user.PasswordHash = Hashing.Hash(model.PasswordHash);
                        user.ResetPasswordExpiry = null;
                        user.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        
                    }
                    else
                    {
                        message = "Reset Link Expired";
                        return View("LinkExpired");
                    }
                }
                else
                {
                    message = "Invalid reset link.";
                    return View("LinkExpired");
                }
            }
            else
            {
                message = "Invalid data.";
                return View(model);
            }


            ViewBag.Message = message;

            TempData["UserReset"] = "<script>Swal.fire({icon: 'success', title: 'Reset Password Success!'});</script>";

            //Redirect to Login
            return RedirectToAction("Login", "Users");
            //return View(model);
        }

        public ActionResult LinkExpired()
        {
            return View();
        }

        public ActionResult Logout([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive,ConfirmPassword,DEPT_ID,DEPT1")] User user)
        {
            var existingUser = db.Users.SingleOrDefault(x => x.UserId == user.UserId);

            if (Session["Userid"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

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
            var cookie = new HttpCookie("ASP.NET_SessionId", "");
            cookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

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

        public ActionResult Dashboard()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            return View();
        }

        [HttpGet]
        public ActionResult TemplateLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TemplateLogin([Bind(Include = "UserId,Username,PasswordHash,Email,CreatedDate,IsActive,ConfirmPassword")] MyLogin user)
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

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    ModelState.AddModelError("Email", "Email field is empty");
                }
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    ModelState.AddModelError("PasswordHash", "Password field is empty");
                }

                return View();
            }

            var hashedPassword = Hashing.Hash(user.PasswordHash);
            var query = db.Users.SingleOrDefault(x => x.Email == user.Email && x.PasswordHash == hashedPassword);

            if (query != null)
            {
                Session["UserId"] = query.UserId;
                Session["Username"] = query.Username.ToString();
                Session["Email"] = query.Email.ToString();

                TempData["user_id"] = query.UserId;

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


