using collegeAdmission.Models;
using Dapper;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.Data.SqlClient;
using System.Reflection;
using System.Net.Mail;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Org.BouncyCastle.Crypto.Macs;
namespace collegeAdmission.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private readonly object BCrypt;
        private string _connectionString;
       
               
           
       
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                   
                    var query = "SELECT ResetToken, ResetTokenExpiry FROM Users WHERE EmailAddress = @EmailAddress";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmailAddress", model.Email);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var token = reader["ResetToken"]?.ToString();
                                var expiry = (DateTime?)reader["ResetTokenExpiry"];

                                if (token == model.Token && expiry > DateTime.Now)
                                {
                                    // Update the password
                                    //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                                    var updateQuery = "UPDATE Users SET Password = @Password, ResetToken = NULL, ResetTokenExpiry = NULL WHERE Email = @EmailAddress";
                                    using (var updateCommand = new SqlCommand(updateQuery, connection))
                                    {
                                        //updateCommand.Parameters.AddWithValue("@Password", Password);
                                        updateCommand.Parameters.AddWithValue("@EmailAddress", model.Email);
                                        updateCommand.ExecuteNonQuery();
                                    }

                                    return RedirectToAction("ResetPasswordConfirmation");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid or expired token.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid email.");
                            }
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(Enquiry model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FirstName", model.FirstName);
                    parameters.Add("@LastName", model.LastName);
                    parameters.Add("@Gmail", model.Gmail);
                    parameters.Add("@Contact", model.Contact);
                    parameters.Add("@Message", model.Message);
                    await connection.ExecuteAsync("ContactEnquiries_user", parameters, commandType: CommandType.StoredProcedure);
                }
               
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Course()
        {
            

            return View();
        }
            
        

        [HttpGet]
        public async Task<IActionResult> Notice()
        {
            List<PublicNotice> publicNotices = new List<PublicNotice>();

            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = "SELECT * FROM PublicNotices";

                    publicNotices = (await connection.QueryAsync<PublicNotice>(query)).ToList();
                }
            }

            return View(publicNotices);
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
                string passwordHash = hasher.HashPassword(null, model.Password);

                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FirstName", model.FirstName);
                    parameters.Add("@LastName", model.LastName);
                    parameters.Add("@DateOfBirth", model.DateOfBirth);
                    parameters.Add("@Gender", model.Gender);
                    parameters.Add("@Contact", model.Contact);
                    parameters.Add("@EmailAddress", model.EmailAddress);
                    parameters.Add("@Address", model.Address);
                    parameters.Add("@State", model.State);
                    parameters.Add("@City", model.City);
                    parameters.Add("@Username", model.Username);
                    parameters.Add("@PasswordHash", model.Password);
                    parameters.Add("@Role", "User");

                    await connection.ExecuteAsync("sp_RegisterUser", parameters, commandType: CommandType.StoredProcedure);
                }


                return RedirectToAction("Login");
            }


            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string UserName, string newPassword, string confirmPassword)
        {
            if(newPassword != confirmPassword)
        {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdatePasswordByUsername", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", UserName); // Use UserId
                        cmd.Parameters.AddWithValue("@NewPassword", HashPassword(newPassword)); // Hash password here

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                ViewBag.SuccessMessage = "Password updated successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the password: " + ex.Message;
            }

            return View();
        }

        private string HashPassword(string password)
        {
            // Implement your hashing logic here (e.g., BCrypt, SHA256).
            return password; // Replace this with hashed pas
        }

            //        return View();
            //    }
            //private void SendEmail(string to, string subject, string body)
            //{
            //    using (var smtp = new SmtpClient("smtp.your-email-provider.com"))
            //    {
            //        var mail = new MailMessage
            //        {
            //            From = new MailAddress("sivasuriya141@gmail.com"),
            //            Subject = subject,
            //            Body = body,
            //            IsBodyHtml = true
            //        };
            //        mail.To.Add("suriyaprakashgmail.com");

            //        smtp.Credentials = new System.Net.NetworkCredential("sivasuriya141@gmail.com", "suriy@pr@k@sh");
            //        smtp.Port = 587; // Adjust for your provider
            //        smtp.EnableSsl = true;
            //        smtp.Send(mail);
            //    }
            //}


            [HttpPost]
        public async Task<IActionResult> UpdateProfile(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@FirstName", model.FirstName);
                    parameters.Add("@LastName", model.LastName);
                    parameters.Add("@DateOfBirth", model.DateOfBirth);
                    parameters.Add("@Gender", model.Gender);
                    parameters.Add("@Contact", model.Contact);
                    parameters.Add("@EmailAddress", model.EmailAddress);
                    parameters.Add("@Address", model.Address);
                    parameters.Add("@State", model.State);
                    parameters.Add("@City", model.City);
                    parameters.Add("@Username", model.Username);
                    parameters.Add("@PasswordHash", model.Password);
                    parameters.Add("@Role", "User");

                    await connection.ExecuteAsync("sp_RegisterUser", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult UserCourses()
        {
            var course = new List<Course>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand("GetActiveCourse", connection) { CommandType = CommandType.StoredProcedure };
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    course.Add(new Course
                    {
                        CourseId = (int)reader["CourseId"],
                        CourseName = reader["CourseName"].ToString(),
                        Description = reader["Description"].ToString(),
                        CreatedAt = (DateTime)reader["CreatedAt"]
                    });
                }
            }
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string role = string.Empty;
            int userId = 0;
            string username  = string.Empty;
            string lName = string.Empty;
            string contact = string.Empty;
            string EmailAddress = string.Empty;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("sp_AuthenticateUser", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@PasswordHash", model.Password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                role = reader["Role"].ToString();
                                userId = Convert.ToInt32(reader["UserId"]);
                                username = Convert.ToString(reader["FirstName"]);
                                lName = Convert.ToString(reader["LastName"]);
                                contact = Convert.ToString(reader["Contact"]);
                                EmailAddress = Convert.ToString(reader["EmailAddress"]);
                            }
                            HttpContext.Session.SetInt32("UserId", userId);
                            HttpContext.Session.SetString("Username", username);
                            HttpContext.Session.SetString("LastName", lName);
                            HttpContext.Session.SetString("Contact", contact);
                            HttpContext.Session.SetString("Email", EmailAddress);

                            if (role == "Admin")
                            {
                                return RedirectToAction("Log", "Admin");
                            }
                            else
                            {
                                return RedirectToAction("Dashboard", "Admin");
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Username or password is incorrect!";
                            return RedirectToAction("Login", "User");
                        }

                    }

                }
            }
        }
    }
}
