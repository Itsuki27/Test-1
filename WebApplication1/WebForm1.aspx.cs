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

        
        SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString);


        protected void Page_Load(object sender, EventArgs e)
        {
            //var context = new MyStartDBEntities();
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
    }
}