using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    public class CoursePageController : Controller
    {
        private readonly string _connectionString;

        public CoursePageController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        // LIST
        public IActionResult List()
        {
            var courses = new List<Course>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM courses", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                courses.Add(new Course
                {
                    CourseId = reader.GetInt32("courseid"),
                    CourseCode = reader.IsDBNull(reader.GetOrdinal("coursecode")) ? null : reader.GetString("coursecode"),
                    TeacherId = reader.IsDBNull(reader.GetOrdinal("teacherid")) ? null : reader.GetInt64("teacherid"),
                    StartDate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime("startdate"),
                    FinishDate = reader.IsDBNull(reader.GetOrdinal("finishdate")) ? (DateTime?)null : reader.GetDateTime("finishdate"),
                    CourseName = reader.IsDBNull(reader.GetOrdinal("coursename")) ? null : reader.GetString("coursename")
                });
            }
            return View(courses);
        }

        // SHOW
        public IActionResult Show(int id)
        {
            var course = GetCourseById(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // CREATE GET
        public IActionResult Create()
        {
            LoadTeachers();
            return View(new Course()); // Pass empty Course to avoid NullReferenceException
        }


        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            var validation = ValidateCourse(course);
            if (validation != null)
            {
                ModelState.AddModelError("", validation);
                LoadTeachers();
                return View(course);
            }

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO courses (coursecode, teacherid, startdate, finishdate, coursename) " +
                "VALUES (@code, @teacherId, @start, @finish, @name)", conn);

            cmd.Parameters.AddWithValue("@code", course.CourseCode);
            cmd.Parameters.AddWithValue("@teacherId", course.TeacherId.HasValue ? (object)course.TeacherId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@start", course.StartDate.HasValue ? (object)course.StartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@finish", course.FinishDate.HasValue ? (object)course.FinishDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@name", course.CourseName);

            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var course = GetCourseById(id);
            if (course == null) return NotFound();

            LoadTeachers(); // Load teachers for the dropdown
            return View(course);
        }


        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course course)
        {
            if (id != course.CourseId)
            {
                ModelState.AddModelError("", "Mismatched Course ID.");
                LoadTeachers();
                return View(course);
            }

            var validation = ValidateCourse(course);
            if (validation != null)
            {
                ModelState.AddModelError("", validation);
                LoadTeachers();
                return View(course);
            }

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "UPDATE courses SET coursecode=@code, teacherid=@teacherId, startdate=@start, finishdate=@finish, coursename=@name WHERE courseid=@id", conn);

            cmd.Parameters.AddWithValue("@code", course.CourseCode);
            cmd.Parameters.AddWithValue("@teacherId", course.TeacherId.HasValue ? (object)course.TeacherId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@start", course.StartDate.HasValue ? (object)course.StartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@finish", course.FinishDate.HasValue ? (object)course.FinishDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@name", course.CourseName);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // DELETE GET
        public IActionResult Delete(int id)
        {
            var course = GetCourseById(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM courses WHERE courseid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // ================= Helper Methods =================

        private Course? GetCourseById(int id)
        {
            if (id <= 0) return null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM courses WHERE courseid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Course
                {
                    CourseId = reader.GetInt32("courseid"),
                    CourseCode = reader.IsDBNull(reader.GetOrdinal("coursecode")) ? null : reader.GetString("coursecode"),
                    TeacherId = reader.IsDBNull(reader.GetOrdinal("teacherid")) ? null : reader.GetInt64("teacherid"),
                    StartDate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime("startdate"),
                    FinishDate = reader.IsDBNull(reader.GetOrdinal("finishdate")) ? (DateTime?)null : reader.GetDateTime("finishdate"),
                    CourseName = reader.IsDBNull(reader.GetOrdinal("coursename")) ? null : reader.GetString("coursename")
                };
            }
            return null;
        }

        private void LoadTeachers()
        {
            var teachers = new List<SelectListItem>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT teacherid, teacherfname, teacherlname FROM teachers", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                teachers.Add(new SelectListItem
                {
                    Value = reader.GetInt64("teacherid").ToString(),
                    Text = $"{reader.GetString("teacherfname")} {reader.GetString("teacherlname")}"
                });
            }
            ViewBag.Teachers = teachers;
        }

        private string? ValidateCourse(Course course)
        {
            if (course == null) return "Course payload is required.";
            if (string.IsNullOrWhiteSpace(course.CourseCode)) return "Course Code is required.";
            if (string.IsNullOrWhiteSpace(course.CourseName)) return "Course Name is required.";
            if (course.TeacherId.HasValue && course.TeacherId <= 0) return "TeacherId must be greater than 0.";
            if (course.StartDate.HasValue && course.FinishDate.HasValue && course.StartDate > course.FinishDate)
                return "StartDate cannot be after FinishDate.";
            return null;
        }
    }
}
