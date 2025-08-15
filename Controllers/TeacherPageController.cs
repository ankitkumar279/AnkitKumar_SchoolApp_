using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly string _connectionString;

        public TeacherPageController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        public async Task<IActionResult> List()
        {
            var teachers = new List<Teacher>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM teachers", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                teachers.Add(new Teacher
                {
                    TeacherId = reader.GetInt32("teacherid"),
                    TeacherFName = reader.IsDBNull(reader.GetOrdinal("teacherfname")) ? null : reader.GetString("teacherfname"),
                    TeacherLName = reader.IsDBNull(reader.GetOrdinal("teacherlname")) ? null : reader.GetString("teacherlname"),
                    EmployeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) ? null : reader.GetString("employeenumber"),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate")) ? (DateTime?)null : reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? (decimal?)null : reader.GetDecimal("salary")
                });
            }

            return View(teachers);
        }

        public async Task<IActionResult> Show(int id)
        {
            if (id <= 0) return NotFound();

            Teacher teacher = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                teacher = new Teacher
                {
                    TeacherId = reader.GetInt32("teacherid"),
                    TeacherFName = reader.IsDBNull(reader.GetOrdinal("teacherfname")) ? null : reader.GetString("teacherfname"),
                    TeacherLName = reader.IsDBNull(reader.GetOrdinal("teacherlname")) ? null : reader.GetString("teacherlname"),
                    EmployeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) ? null : reader.GetString("employeenumber"),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate")) ? (DateTime?)null : reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? (decimal?)null : reader.GetDecimal("salary")
                };
            }

            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // Create GET
        public IActionResult Create() => View();

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Teacher teacher)
        {
            ApplyServerSideValidations(teacher);

            if (!ModelState.IsValid) return View(teacher);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                "VALUES (@teacherfname, @teacherlname, @EmployeeNumber, @hiredate, @salary)", conn);

            cmd.Parameters.AddWithValue("@teacherfname", (object?)teacher.TeacherFName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@teacherlname", (object?)teacher.TeacherLName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeNumber", (object?)teacher.EmployeeNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@hiredate", (object?)teacher.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@salary", (object?)teacher.Salary ?? DBNull.Value);

            cmd.ExecuteNonQuery();
            return RedirectToAction(nameof(List));
        }

        // Edit GET
        public IActionResult Edit(int id)
        {
            if (id <= 0) return NotFound();

            Teacher teacher = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                teacher = new Teacher
                {
                    TeacherId = reader.GetInt32("teacherid"),
                    TeacherFName = reader.IsDBNull(reader.GetOrdinal("teacherfname")) ? null : reader.GetString("teacherfname"),
                    TeacherLName = reader.IsDBNull(reader.GetOrdinal("teacherlname")) ? null : reader.GetString("teacherlname"),
                    EmployeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) ? null : reader.GetString("employeenumber"),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate")) ? (DateTime?)null : reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? (decimal?)null : reader.GetDecimal("salary")
                };
            }

            return teacher == null ? NotFound() : View(teacher);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                ModelState.AddModelError("", "Mismatched Teacher ID.");
            }

            ApplyServerSideValidations(teacher);

            if (!ModelState.IsValid) return View(teacher);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Ensure teacher exists
            using (var existsCmd = new MySqlCommand("SELECT COUNT(*) FROM teachers WHERE teacherid=@id", conn))
            {
                existsCmd.Parameters.AddWithValue("@id", id);
                var count = Convert.ToInt32(existsCmd.ExecuteScalar());
                if (count == 0)
                {
                    ModelState.AddModelError("", "Teacher not found.");
                    return View(teacher);
                }
            }

            var cmd = new MySqlCommand(
                "UPDATE teachers " +
                "SET teacherfname=@TeacherFName, teacherlname=@TeacherLName, employeenumber=@EmployeeNumber, hiredate=@HireDate, salary=@Salary " +
                "WHERE teacherid=@id", conn);

            cmd.Parameters.AddWithValue("@TeacherFName", (object?)teacher.TeacherFName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TeacherLName", (object?)teacher.TeacherLName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeNumber", (object?)teacher.EmployeeNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", (object?)teacher.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Salary", (object?)teacher.Salary ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
            {
                ModelState.AddModelError("", "Update failed.");
                return View(teacher);
            }

            return RedirectToAction(nameof(List));
        }

        // Delete GET
        public IActionResult Delete(int id)
        {
            if (id <= 0) return NotFound();

            Teacher teacher = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("SELECT * FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                teacher = new Teacher
                {
                    TeacherId = reader.GetInt32("teacherid"),
                    TeacherFName = reader.IsDBNull(reader.GetOrdinal("teacherfname")) ? null : reader.GetString("teacherfname"),
                    TeacherLName = reader.IsDBNull(reader.GetOrdinal("teacherlname")) ? null : reader.GetString("teacherlname"),
                    EmployeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) ? null : reader.GetString("employeenumber"),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate")) ? (DateTime?)null : reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? (decimal?)null : reader.GetDecimal("salary")
                };
            }

            return teacher == null ? NotFound() : View(teacher);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToAction(nameof(List));
        }

        /// <summary>
        /// Server-side validations to earn initiative marks.
        /// </summary>
        private void ApplyServerSideValidations(Teacher teacher)
        {
            if (teacher is null)
            {
                ModelState.AddModelError("", "Teacher payload is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(teacher.TeacherFName))
                ModelState.AddModelError(nameof(teacher.TeacherFName), "First name is required.");

            if (string.IsNullOrWhiteSpace(teacher.TeacherLName))
                ModelState.AddModelError(nameof(teacher.TeacherLName), "Last name is required.");

            if (teacher.HireDate.HasValue && teacher.HireDate.Value.Date > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(teacher.HireDate), "Hire date cannot be in the future.");

            if (teacher.Salary.HasValue && teacher.Salary.Value < 0)
                ModelState.AddModelError(nameof(teacher.Salary), "Salary must be 0 or greater.");
        }
    }
}
