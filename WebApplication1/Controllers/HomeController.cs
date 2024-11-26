using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
//using System.Configuration;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        //private string _connectionString = "data source=DESKTOP-0CV5V0K;initial catalog=MyStartDB;persist security info=True;user id=sa;password=sa123;encrypt=False;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework";

        //public ActionResult CheckDatabase()
        //{
        //    bool isConnected = false;
        //    string errorMessage = string.Empty;

        //    try
        //    {
        //        using (var connection = new SqlConnection(_connectionString))
        //        {
        //            connection.Open(); // Try to open the connection

        //            // Test if the database exists by running a simple query.
        //            var command = new SqlCommand("DESKTOP-0CV5VOK", connection);
        //            var result = command.ExecuteScalar();

        //            if (result != null)
        //            {
        //                isConnected = true; // Database exists and the connection is valid
        //            }

        //            connection.Close(); // Close the connection
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        isConnected = false; // Connection failed
        //        errorMessage = ex.Message; // Capture the error message
        //    }

        //    if (isConnected)
        //    {
        //        Response.Write("<script>alert('Database connection is valid. Connected to " + errorMessage + "')</script>");
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert('Wrong database: " + errorMessage + "')</script>");
        //    }

        //    return View();
        //}

        //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyStartDBEntities"].ConnectionString);



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {

            return View();
        }
        public ActionResult Reports()
        {

            return View();
        }
    }
}