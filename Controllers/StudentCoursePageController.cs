using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    public class StudentCoursePageController : Controller
    {
        private readonly string _connectionString;

        public StudentCoursePageController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        // LIST
        public IActionResult List()
        {
            var list = new List<StudentCourse>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT * FROM studentsxcourses", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new StudentCourse
                {
                    StudentXCourseId = reader.GetInt32("studentxcoursid"),
                    StudentId = reader.GetInt32("studentid"),
                    CourseId = reader.GetInt32("courseid")
                });
            }
            return View(list);
        }

        // SHOW
        public IActionResult Show(int id)
        {
            var sc = GetStudentCourseById(id);
            return sc == null ? NotFound() : View(sc);
        }

        // CREATE GET
        public IActionResult Create()
        {
            PopulateViewBags();
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentCourse sc)
        {
            if (sc.StudentId <= 0 || sc.CourseId <= 0)
            {
                ModelState.AddModelError("", "Please select both Student and Course.");
            }

            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return View(sc);
            }

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO studentsxcourses (studentid, courseid) VALUES (@studentid, @courseid)", conn);
            cmd.Parameters.AddWithValue("@studentid", sc.StudentId);
            cmd.Parameters.AddWithValue("@courseid", sc.CourseId);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(List));
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var sc = GetStudentCourseById(id);
            if (sc == null) return NotFound();

            PopulateViewBags();
            return View(sc);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, StudentCourse sc)
        {
            if (sc.StudentId <= 0 || sc.CourseId <= 0)
            {
                ModelState.AddModelError("", "Please select both Student and Course.");
            }

            if (id != sc.StudentXCourseId) ModelState.AddModelError("", "ID mismatch.");

            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return View(sc);
            }

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE studentsxcourses SET studentid=@studentid, courseid=@courseid WHERE studentxcoursid=@id", conn);
            cmd.Parameters.AddWithValue("@studentid", sc.StudentId);
            cmd.Parameters.AddWithValue("@courseid", sc.CourseId);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(List));
        }

        // DELETE GET
        public IActionResult Delete(int id)
        {
            var sc = GetStudentCourseById(id);
            return sc == null ? NotFound() : View(sc);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM studentsxcourses WHERE studentxcoursid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // ================= Helper Methods =================

        private StudentCourse GetStudentCourseById(int id)
        {
            StudentCourse sc = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT * FROM studentsxcourses WHERE studentxcoursid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sc = new StudentCourse
                {
                    StudentXCourseId = reader.GetInt32("studentxcoursid"),
                    StudentId = reader.GetInt32("studentid"),
                    CourseId = reader.GetInt32("courseid")
                };
            }
            return sc;
        }

        private void PopulateViewBags()
        {
            // Students
            var students = new List<SelectListItem>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using var studentsCmd = new MySqlCommand(
                    "SELECT studentid, CONCAT(studentfname, ' ', studentlname) AS Name FROM students", conn);
                using var reader = studentsCmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new SelectListItem
                    {
                        Value = reader.GetInt32("studentid").ToString(),
                        Text = reader.GetString("Name")
                    });
                }
            }
            ViewBag.Students = students;

            // Courses
            var courses = new List<SelectListItem>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using var coursesCmd = new MySqlCommand(
                    "SELECT courseid, CONCAT(coursecode, ' - ', coursename) AS Name FROM courses", conn);
                using var reader = coursesCmd.ExecuteReader();
                while (reader.Read())
                {
                    courses.Add(new SelectListItem
                    {
                        Value = reader.GetInt32("courseid").ToString(),
                        Text = reader.GetString("Name")
                    });
                }
            }
            ViewBag.Courses = courses;
        }
    }
}
