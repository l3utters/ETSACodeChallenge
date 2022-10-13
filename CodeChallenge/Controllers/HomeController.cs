using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;
using System.IO;

namespace CodeChallenge.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateUser()
        {
            ViewBag.Message = "Create a new database user";

            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(CreateUser user)
        {
            string username = user.Username;
            string password = user.Password;

            CreateUser(username, password);

            return View();
        }

        private void CreateUser(string username, string password)
        {
            if(username != "" || password != "")
            {
                string connetionString = "Server= localhost; Database= ETSASkillTest; Integrated Security=True;";
                SqlConnection con = new SqlConnection(connetionString);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "INSERT INTO est_Users(UserId, UserName, Password, PasswordSalt, CreationDate, UserStatusNo) VALUES(NEWID(),'" + username + "', HASHBYTES('SHA2_256','" + password + "'), PWDENCRYPT('" + password + "'), GETDATE(), 2)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public ActionResult UpdateUser()
        {
            ViewBag.Message = "Update information of a current user";

            return View();
        }

        [HttpPost]
        public ActionResult UpdateUser(string Username, int Status)
        {
            string username = Username;
            int status = Status;

            string connetionString = "Server= localhost; Database= ETSASkillTest; Integrated Security=True;";
            SqlConnection con = new SqlConnection(connetionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE est_Users SET UserStatusNo = "+status+" WHERE UserName = '"+username+"'";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            return View();
        }

        public ActionResult DeleteUser()
        {
            ViewBag.Message = "Delete an existing user";

            return View();
        }

        [HttpPost]
        public ActionResult DeleteUser(string Username, string Confirmation)
        {
            if (Confirmation == "DELETE" && Username != "")
            {
                string connetionString = "Server= localhost; Database= ETSASkillTest; Integrated Security=True;";
                SqlConnection con = new SqlConnection(connetionString);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "DELETE FROM est_Users WHERE UserName = '"+ Username +"'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return View();
        }

        public ActionResult UserLogin()
        {
            ViewBag.Message = "Login page for users";

            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(string Username, string Password)
        {

            string connetionString = "Server= localhost; Database= ETSASkillTest; Integrated Security=True;";
            SqlConnection con = new SqlConnection(connetionString);
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT UserName, Password FROM est_Users WHERE UserName = '"+Username+"' AND Password = HASHBYTES('SHA2_256','"+Password+"')", con);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                return Content("You are now logged in");
            }
            else
            {
                return Content("Ensure Username and Password are Correct");
            }
            
        }

        public ActionResult ExportActiveUsers()
        {
            string connetionString = "Server= localhost; Database= ETSASkillTest; Integrated Security=True;";
            SqlConnection con = new SqlConnection(connetionString);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select UserName from est_Users WHERE UserStatusNo = 1", con);
            SqlDataReader reader = cmd.ExecuteReader();

            string fileName = "C:\\Users\\Phillip-Laptop\\Desktop\\ActiveUsers.csv";
            StreamWriter sw = new StreamWriter(fileName);
            object[] output = new object[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
                output[i] = reader.GetName(i);

            sw.WriteLine(string.Join(",", output));

            while (reader.Read())
            {
                reader.GetValues(output);
                sw.WriteLine(string.Join(",", output));
            }

            sw.Close();
            reader.Close();
            con.Close();

            byte[] fileBytes = System.IO.File.ReadAllBytes(@fileName);
            string file = "ActiveUsers.csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file);
        }
    }
}