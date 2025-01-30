using System;
using System.Data;
using System.Net.Mail;
using collegeAdmission.Models;

using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace collegeAdmission.Controllers
{
    public class AdminController : Controller
    {

        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        [HttpGet]
        public IActionResult Login()
        {
            var user = HttpContext.User;
            return View();

        }

        public HttpContext GetHttpContext()
        {
            return HttpContext;
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (model.Username == "admin" && model.Password == "admin123")
                {
                    HttpContext.Session.SetString("Admin", "Authenticated");
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }
            return View(model);
        }


        //public async Task<IActionResult> Courses()
        //{
        //    var Programs = await _courseService.GetAllCoursesAsync();
        //    return View(Programs);
        //}

        //[HttpPost]
        //public async Task<IActionResult> ToggleVisibility(int courseId)
        //{
        //    await _courseService.ToggleCourseVisibilityAsync(courseId);
        //    return RedirectToAction("Courses");
        //}


        [HttpGet]
        public IActionResult Dashboard()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewData["showAlert"] = false;
            return View();

        }


        //List<StatusAppilication> lststudents = new List<StatusAppilication>();

        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        connection.Open();
        //        string query = "SELECT Id, name, phone, program, course, address, created_at ,AdmissionStatus FROM AdmissionForm" +
        //            " Where email= @email";
        //        SqlCommand cmd = new SqlCommand(query, connection);
        //        cmd.Parameters.AddWithValue("@email", HttpContext.Session.GetString("Email"));
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            lststudents.Add(new StatusAppilication
        //            {
        //                Id = reader.GetInt32(0),
        //                Name = reader.GetString(1),
        //                Phone = reader.GetString(2),
        //                Program = reader.GetString(3),
        //                Course = reader.GetString(4),
        //                Address = reader.GetString(5),
        //                CreatedDate = reader.GetDateTime(6),
        //                Status = reader.GetString(7)
        //            });
        //        }
        //    }





        public IActionResult AboutUs()
        {
            
            return View();
        }

        public IActionResult Log()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }


        public IActionResult Profile()
        {
            var firstName = HttpContext.Session.GetString("Username");
            var lastName = HttpContext.Session.GetString("LastName");
            var contact = HttpContext.Session.GetString("Contact");
            var email = HttpContext.Session.GetString("Email");
            int? userID = HttpContext.Session.GetInt32("UserId");

            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.Contact = contact;
            ViewBag.Email = email;


            //var model = new ProfileViewModel
            //{
            //    FirstName = firstName,
            //    LastName = lastName,
            //    Phone = contact,
            //    Email = email,
            //    UserID = userID
            //};

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Login");
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel model)
        {
            int? userID = HttpContext.Session.GetInt32("UserId");
            var firstName = HttpContext.Session.GetString("Username");
            var lastName = HttpContext.Session.GetString("LastName");
            var contact = HttpContext.Session.GetString("Contact");
            var email = HttpContext.Session.GetString("Email");
            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (var command = new SqlCommand("spUpadateProfile", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar) { Value = model.FirstName });
                        command.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar) { Value = model.LastName });
                        command.Parameters.Add(new SqlParameter("@Contact", SqlDbType.NVarChar) { Value = model.Phone });
                        command.Parameters.Add(new SqlParameter("@EmailAddress", SqlDbType.NVarChar) { Value = model.Email });

                        connection.Open();

                        int result = command.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["SuccessMessage"] = "Profile updated successfully!";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Profile update failed. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while updating your profile: {ex.Message}";
            }


            return View(model);
        }

        //[HttpPost]
        //public IActionResult AdmissionForm(FormViewModel model)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            using (var command = new SqlCommand("InsertAdmissionForm", connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;

        //                command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar) { Value = model.FullName });
        //                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar) { Value = model.PhoneNumber });
        //                command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar) { Value = model.Email });
        //                command.Parameters.Add(new SqlParameter("@address", SqlDbType.NVarChar) { Value = model.Address });
        //                command.Parameters.Add(new SqlParameter("@program", SqlDbType.NVarChar) { Value = model.Program });
        //                command.Parameters.Add(new SqlParameter("@course", SqlDbType.NVarChar) { Value = model.Course });
        //                command.Parameters.Add(new SqlParameter("@marksheet10", SqlDbType.NVarChar) { Value = model.Marksheet10Path });
        //                command.Parameters.Add(new SqlParameter("@marksheet12", SqlDbType.NVarChar) { Value = model.Marksheet12Path });
        //                command.Parameters.Add(new SqlParameter("@ug_marksheet", SqlDbType.NVarChar) { Value = model.UGMarksheetPath });

        //                connection.Open();

        //                int result = command.ExecuteNonQuery();
        //                if (result > 0)
        //                {
        //                    TempData["SuccessMessage"] = "Profile updated successfully!";
        //                }
        //                else
        //                {
        //                    TempData["ErrorMessage"] = "Profile update failed. Please try again.";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = $"An error occurred while updating your profile: {ex.Message}";
        //    }
        //    return View(model);
        //}
        //        [HttpPost]
        //        public async Task<IActionResult> form(FormViewModel model)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                try
        //                {
        //                    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //                    {
        //                        var parameters = new DynamicParameters();
        //                        parameters.Add("@name", model.FullName);
        //                        parameters.Add("@phone", model.PhoneNumber);
        //                        parameters.Add("@email", model.Email);
        //                        parameters.Add("@address", model.Address);
        //                        parameters.Add("@program", model.Program);
        //                        parameters.Add("@course", model.Course);
        //                        parameters.Add("@marksheet10", model.Marksheet10Path ?? (object)DBNull.Value);
        //                        parameters.Add("@marksheet12", model.Marksheet12Path ?? (object)DBNull.Value);
        //                        parameters.Add("@ug_marksheet", model.UGMarksheetPath ?? (object)DBNull.Value);

        //                        var result = await connection.ExecuteAsync("InsertAdmissionForm", parameters, commandType: CommandType.StoredProcedure);

        //                        // Check result to confirm the operation succeeded
        //                        if (result > 0)
        //                        {
        //                            return RedirectToAction("Success");
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Log the exception for debugging
        //                    Console.WriteLine($"Error: {ex.Message}");
        //                    ModelState.AddModelError("", "An error occurred while saving the data.");
        //                }
        //            }

        //            return View(model);

        //       }
        //        public IActionResult Success()
        //        {
        //            return View();
        //        }

        //[HttpPost]
        //public IActionResult AdmissionForm(FormViewModel model , IFormFile marksheet10, IFormFile marksheet12, IFormFile ugMarksheet)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //            {
        //                using (var command = new SqlCommand("InsertAdmissionForm", connection))
        //                {
        //                    command.CommandType = CommandType.StoredProcedure;

        //                    // Add parameters
        //                    command.Parameters.AddWithValue("@name", model.FullName);
        //                    command.Parameters.AddWithValue("@email", model.Email);
        //                    command.Parameters.AddWithValue("@phone", model.PhoneNumber);
        //                    command.Parameters.AddWithValue("@address", model.Address);
        //                    command.Parameters.AddWithValue("@program", model.Program);
        //                    command.Parameters.AddWithValue("@course", model.Course);
        //                    command.Parameters.AddWithValue("@marksheet10", model.Marksheet10);
        //                    command.Parameters.AddWithValue("@marksheet12", model.Marksheet12);
        //                    command.Parameters.AddWithValue("@ug_marksheet", model.UGMarksheet ?? (object)DBNull.Value);

        //                    connection.Open();
        //                    int rowsAffected = command.ExecuteNonQuery();

        //                    if (rowsAffected > 0)
        //                    {
        //                        TempData["SuccessMessage"] = "Admission form submitted successfully!";
        //                    }
        //                    else
        //                    {
        //                        TempData["ErrorMessage"] = "Failed to submit the admission form.";
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        //        }
        //    }

        //    return View(model);
        //}

        [HttpGet]
        public IActionResult Course()
        {
            return View();
        }


        [HttpGet]
        public IActionResult SubmitFees()
        {

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string email = Convert.ToString(HttpContext.Session.GetString("Email"));
                string query = "SELECT COUNT(*) FROM AdmissionForm where email = @email AND Status = 'Accepted'";
                var command = new SqlCommand(query, connection);

                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    int result = Convert.ToInt32(command.ExecuteScalar());

                    if(result != 0)
                    {
                        return View(new FeesViewModel());

                    }
                }
            }
            TempData["AlertMessage"] = "You have not been accepted yet. Please complete your application to proceed.";
            ViewData["showAlert"] = true;
            return RedirectToAction("Dashboard","Admin");
        }

        //public void AddInquiry(string firstName, string lastName, string gmail, string contact, string message)
        //{
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("AddInquiryDetails", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@FirstName", firstName);
        //            cmd.Parameters.AddWithValue("@LastName", lastName);
        //            cmd.Parameters.AddWithValue("@Gmail", gmail);
        //            cmd.Parameters.AddWithValue("@Contact", contact);
        //            cmd.Parameters.AddWithValue("@Message", message);

        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SubmitFees(FeesViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        var command = new SqlCommand("InsertFeesSubmission", connection)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        command.Parameters.AddWithValue("@StudentName", model.StudentName);
                        command.Parameters.AddWithValue("@FeeAmount", model.FeeAmount);
                        command.Parameters.AddWithValue("@PaymentMode", model.PaymentMode);
                        command.Parameters.AddWithValue("@PaymentDate", model.PaymentDate);

                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                    }

                    TempData["SuccessMessage"] = "Fees submitted successfully!";
                    return RedirectToAction("SubmitFees");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AdmissionForms()
        {
            var user = HttpContext.User;
            return View();
            
        }
        private async Task<byte[]> ConvertToByteArray(IFormFile file)
        {
            if (file == null)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SubmitAdmissionForm(AdmissionForm admissionForm)
        {
            if (ModelState.IsValid)
            {
                // Save the admission form data in the session
                HttpContext.Session.SetString("FullName", admissionForm.FullName);
                HttpContext.Session.SetString("Status", admissionForm.Status);
              

                // Redirect to the page where you display the submitted data
                return RedirectToAction("ApplicationStatus");
            }

            return View("Apply", admissionForm); // Return to the form if validation fails
        }
        [HttpGet]
        public IActionResult ApplicationStatus()
        {
            // Retrieve the admission form data from session
            var fullName = HttpContext.Session.GetString("FullName");
            var status = HttpContext.Session.GetString("Status");
            var submittedOn = HttpContext.Session.GetString("SubmittedOn");

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(status) || string.IsNullOrEmpty(submittedOn))
            {
                return View("NoApplication"); // Redirect to a page if no application is found
            }

            var admissionForm = new AdmissionForm
            {
                FullName = fullName,
                Status = status,
                
            };

            return View(admissionForm); // Return the admission form data to the view
        }

        
        

        [HttpPost]
        public async Task<IActionResult> AdmissionForms(AdmissionForm model, IFormFile marksheet10, IFormFile marksheet12, IFormFile ugMarksheet)
        {

            

            byte[] marksheet10Bytes = marksheet10 != null ? await ConvertToByteArray(marksheet10) : null;
            byte[] marksheet12Bytes = marksheet12 != null ? await ConvertToByteArray(marksheet12) : null;
            byte[] ugMarksheetBytes = ugMarksheet != null ? await ConvertToByteArray(ugMarksheet) : null;


            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("InsertAdmissionForm", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.AddWithValue("@Name", model.FullName);
                    command.Parameters.AddWithValue("@Phone", model.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@Program", model.Program);
                    command.Parameters.AddWithValue("@Course", model.Course);
                    command.Parameters.Add(new SqlParameter("@Marksheet10", marksheet10Bytes ?? (object)DBNull.Value) { SqlDbType = SqlDbType.VarBinary });
                    command.Parameters.Add(new SqlParameter("@Marksheet12", marksheet12Bytes ?? (object)DBNull.Value) { SqlDbType = SqlDbType.VarBinary });
                    command.Parameters.Add(new SqlParameter("@UG_Marksheet", ugMarksheetBytes ?? (object)DBNull.Value) { SqlDbType = SqlDbType.VarBinary });

                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Success");


        }
        [HttpGet]
        public IActionResult RegUsers()
        {
            List<User> users = new List<User>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                string query = "SELECT UserId, FirstName, LastName, Username, EmailAddress, Contact, Gender FROM Users";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        UserName = reader.GetString(3),
                        Email = reader.GetString(4),
                        Contact = reader.GetString(5),
                        Gender = reader.GetString(6)
                    });
                }
            }

            return View(users);
        }


        [HttpPost]
        public IActionResult AcceptStudent(int studentId)
        {

            return RedirectToAction("Applications");
        }

        [HttpPost]
        public IActionResult RejectStudent(int studentId)
        {

            return RedirectToAction("Applications");
        }

        [HttpGet]
        public IActionResult Applications()
        {
            List<StudentViewModel> students = new List<StudentViewModel>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                string query = "SELECT Id, name, phone, program, course, address, created_at FROM AdmissionForm";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new StudentViewModel
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Phone = reader.GetString(2),
                        Program = reader.GetString(3),
                        Course = reader.GetString(4),
                        Address = reader.GetString(5),
                        CreatedDate = reader.GetDateTime(6)
                    });
                }
            }

            return View(students);
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddNotice()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNotice(PublicNotice model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Title", model.Title);
                    parameters.Add("@Description", model.Description);
                    await connection.ExecuteAsync("AddPublicNotice", parameters, commandType: CommandType.StoredProcedure);
                }
               
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult EditNotice(int id)
        {
            PublicNotice notice = new PublicNotice();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("GetNoticeById", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NoticeId", id);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        notice.NoticeID = (int)reader["NoticeId"];
                        notice.Title = reader["Title"].ToString();
                        notice.Description = reader["Description"].ToString();
                        
                    }
                    connection.Close();
                }
            }

            return View(notice);
        }

        // Edit Notice - POST
        [HttpPost]
        public IActionResult EditNotice(PublicNotice notice)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateNotice", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NoticeId", notice.NoticeID);
                    cmd.Parameters.AddWithValue("@Title", notice.Title);
                    cmd.Parameters.AddWithValue("@Description", notice.Description);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("NoticeList");
        }
        public IActionResult DeleteNotice(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteNotice", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NoticeId", id);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("NoticeList");
        }



        [HttpGet]
        public async Task<IActionResult> Enquiry()
        {
            List<Enquiry> ContactEnquiries = new List<Enquiry>();

            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = "SELECT * FROM ContactEnquiries";

                    ContactEnquiries = (await connection.QueryAsync<Enquiry>(query)).ToList();
                }
            }

            return View(ContactEnquiries);
        }
        [HttpGet]
        public IActionResult AdminCourses()
        {
            var course = new List<Course>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand("GetAllCourse", connection) { CommandType = CommandType.StoredProcedure };
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    course.Add(new Course
                    {
                        CourseId = reader["CourseId"] != DBNull.Value ? Convert.ToInt32(reader["CourseId"]) : 0,
                        CourseName = reader["CourseName"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString() ?? string.Empty,
                        IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                        CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue

                    });
                }
            }
            return View(course);
        }

        [HttpGet]
        public IActionResult EditCourse(int? id)
        {
            if (id == null) return View(new Course());

            Course course = null;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand("GetCourseById", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@CourseId", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    course = new Course
                    {
                        CourseId = (int)reader["CourseId"],
                        CourseName = reader["CourseName"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = (bool)reader["IsActive"]
                    };
                }
            }
            return View(course);
        }

        
        [HttpPost]
        public IActionResult EditCourse(Course model)
        {
             using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (model.CourseId == 0)
                {
                    var command = new SqlCommand("AddCourse", connection) { CommandType = CommandType.StoredProcedure };
                    command.Parameters.AddWithValue("@CourseName", model.CourseName);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@IsActive", model.IsActive);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                else
                {
                    var command = new SqlCommand("UpdateCourse", connection) { CommandType = CommandType.StoredProcedure };
                    command.Parameters.AddWithValue("@CourseId", model.CourseId);
                    command.Parameters.AddWithValue("@CourseName", model.CourseName);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@IsActive", model.IsActive);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("AdminCourses");
        }

       
        [HttpPost]
        public IActionResult DeleteCourse(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand("DeleteCourse", connection) { CommandType = CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@CourseId", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction("AdminCourses");
        }
        public IActionResult Status()
        {
            var application = new AdmissionForm();
            string email = Convert.ToString(HttpContext.Session.GetString("Email"));

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT Id, name, course,email ,Status  FROM AdmissionForm where email = @email";
                var command = new SqlCommand(query, connection);

                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@email", email);
                    //command.Parameters.AddWithValue("@Status", "In Progress");

                    connection.Open();
                   
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            application.Id = Convert.ToInt32(reader["id"]);
                            application.FullName = reader["name"].ToString();
                            application.Course = reader["Course"].ToString();
                            application.Email = reader["email"].ToString();
                            application.Status = reader["Status"].ToString();

                        }
                    }
                }
            }

            return View(application);
         }
        public IActionResult progress(int id)
        {
            AdmissionForm application = null;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("GetApplicationByUserId", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    application = new AdmissionForm
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        FullName = reader["name"].ToString(),
                        Status = reader["Status"].ToString(),
                       
                    };
                }
            }

            return View(application);
        }
        

            public IActionResult AdminAction()
            {
                List<AdmissionForm> applications = new List<AdmissionForm>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                    SqlCommand cmd = new SqlCommand("GetApplications", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        applications.Add(new AdmissionForm
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            FullName = reader["name"].ToString(),
                            Course = reader["Course"].ToString(),
                            Status = reader["Status"].ToString(),
                        });
                    }
                }

                return View(applications);
            }

            [HttpPost]
            public IActionResult UpdateStatus(int id, string status)
            {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                    SqlCommand cmd = new SqlCommand("UpdateApplicationStats", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@Status", status);

                connection.Open();
                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("AdminAction");
            }
        }


    }








    
    
    









