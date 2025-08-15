using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    public class StudentPageController : Controller
    {
        private readonly string _connectionString;

        public StudentPageController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        // List all students
        public IActionResult List()
        {
            var students = new List<Student>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM students", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    StudentId = reader.GetInt32("studentid"),
                    StudentFName = reader.GetString("studentfname"),
                    StudentLName = reader.GetString("studentlname"),
                    StudentNumber = reader.GetString("studentnumber"),
                    EnrolDate = reader.IsDBNull(reader.GetOrdinal("enroldate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("enroldate")
                });
            }
            return View(students);
        }

        // Show student details
        public IActionResult Show(int id)
        {
            if (id <= 0) return NotFound();
            Student student = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM students WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                student = new Student
                {
                    StudentId = reader.GetInt32("studentid"),
                    StudentFName = reader.GetString("studentfname"),
                    StudentLName = reader.GetString("studentlname"),
                    StudentNumber = reader.GetString("studentnumber"),
                    EnrolDate = reader.IsDBNull(reader.GetOrdinal("enroldate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("enroldate")
                };
            }
            return student == null ? NotFound() : View(student);
        }

        // Create GET
        public IActionResult Create() => View();

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student)
        {
            ApplyServerSideValidations(student);
            if (!ModelState.IsValid) return View(student);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO students (studentfname, studentlname, studentnumber, enroldate) " +
                "VALUES (@fname, @lname, @number, @date)", conn);
            cmd.Parameters.AddWithValue("@fname", student.StudentFName);
            cmd.Parameters.AddWithValue("@lname", student.StudentLName);
            cmd.Parameters.AddWithValue("@number", student.StudentNumber);
            cmd.Parameters.AddWithValue("@date", student.EnrolDate.HasValue ? (object)student.EnrolDate.Value : DBNull.Value);

            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // Edit GET
        public IActionResult Edit(int id)
        {
            if (id <= 0) return NotFound();
            Student student = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM students WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                student = new Student
                {
                    StudentId = reader.GetInt32("studentid"),
                    StudentFName = reader.GetString("studentfname"),
                    StudentLName = reader.GetString("studentlname"),
                    StudentNumber = reader.GetString("studentnumber"),
                    EnrolDate = reader.IsDBNull(reader.GetOrdinal("enroldate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("enroldate")
                };
            }
            return student == null ? NotFound() : View(student);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Student student)
        {
            if (id != student.StudentId)
            {
                ModelState.AddModelError("", "Mismatched Student ID.");
            }

            ApplyServerSideValidations(student);
            if (!ModelState.IsValid) return View(student);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Ensure student exists
            using var existsCmd = new MySqlCommand("SELECT COUNT(*) FROM students WHERE studentid=@id", conn);
            existsCmd.Parameters.AddWithValue("@id", id);
            if (Convert.ToInt32(existsCmd.ExecuteScalar()) == 0)
            {
                ModelState.AddModelError("", "Student not found.");
                return View(student);
            }

            var cmd = new MySqlCommand(
                "UPDATE students SET studentfname=@fname, studentlname=@lname, studentnumber=@number, enroldate=@date WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@fname", student.StudentFName);
            cmd.Parameters.AddWithValue("@lname", student.StudentLName);
            cmd.Parameters.AddWithValue("@number", student.StudentNumber);
            cmd.Parameters.AddWithValue("@date", student.EnrolDate.HasValue ? (object)student.EnrolDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // Delete GET
        public IActionResult Delete(int id)
        {
            if (id <= 0) return NotFound();
            Student student = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM students WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                student = new Student
                {
                    StudentId = reader.GetInt32("studentid"),
                    StudentFName = reader.GetString("studentfname"),
                    StudentLName = reader.GetString("studentlname"),
                    StudentNumber = reader.GetString("studentnumber"),
                    EnrolDate = reader.IsDBNull(reader.GetOrdinal("enroldate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("enroldate")
                };
            }
            return student == null ? NotFound() : View(student);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM students WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        private void ApplyServerSideValidations(Student student)
        {
            if (student == null)
            {
                ModelState.AddModelError("", "Student payload is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(student.StudentFName))
                ModelState.AddModelError(nameof(student.StudentFName), "First name is required.");

            if (string.IsNullOrWhiteSpace(student.StudentLName))
                ModelState.AddModelError(nameof(student.StudentLName), "Last name is required.");

            if (student.EnrolDate.HasValue && student.EnrolDate.Value.Date > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(student.EnrolDate), "Enrollment date cannot be in the future.");
        }
    }
}
