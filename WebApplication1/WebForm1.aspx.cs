using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MyStartDBEntities"].ConnectionString);


        protected void Page_Load(object sender, EventArgs e)
        {
            var context = new MyStartDBEntities();
            Conn.Open();
            if (Conn.State == System.Data.ConnectionState.Open)
            {
                Response.Write("Connection Ok!");
                Conn.Close();
            }
            else
            {
                Response.Write("No Connection!");
                Conn.Close();
            }
        }

        //public void ConnectDB()
        //{
        //    SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MyStartDBEntities"].ConnectionString);

        //    Conn.Open();
        //    if (Conn.State == System.Data.ConnectionState.Open)
        //    {
        //        DisplayMessage(this, "Connection to the Database Successful");
        //    }

        //}
        //static public void DisplayMessage(Control page, string msg)
        //{
        //    string myScript = String.Format("alert('{0}')", msg);
        //    ScriptManager.RegisterStartupScript(page, page.GetType(), "myScript", myScript, true);
        //}


    }
}