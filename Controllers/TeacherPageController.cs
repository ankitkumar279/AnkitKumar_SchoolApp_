using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    public class TeacherPageController : Controller
    {
        //private readonly SchoolDbContext _context;

        //public TeacherPageController(SchoolDbContext context)
        //{
        //    _context = context;
        //}

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
                    TeacherFName = reader.GetString("teacherfname"),
                    TeacherLName = reader.GetString("teacherlname"),
                    EmployeeNumber = reader.GetString("employeenumber"),
                    HireDate = reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : reader.GetDecimal("Salary")

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
                    TeacherFName = reader.GetString("teacherfname"),
                    TeacherLName = reader.GetString("teacherlname"),
                    EmployeeNumber = reader.GetString("employeenumber"),
                    HireDate = reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : reader.GetDecimal("Salary")

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
            if (!ModelState.IsValid) return View(teacher);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@TeacherFName, @TeacherLName, @EmployeeNumber , @HireDate, @Salary)", conn);

            cmd.Parameters.AddWithValue("@teacherfname", teacher.TeacherFName);
            cmd.Parameters.AddWithValue("@teacherlname", teacher.TeacherLName);
            cmd.Parameters.AddWithValue("@EmployeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@hiredate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@salary", teacher.Salary);

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
                    TeacherFName = reader.GetString("teacherfname"),
                    TeacherLName = reader.GetString("teacherlname"),
                    EmployeeNumber = reader.GetString("employeenumber"),
                    HireDate = reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : reader.GetDecimal("Salary")


                };
            }

            return teacher == null ? NotFound() : View(teacher);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Teacher teacher)
        {
            if (id != teacher.TeacherId || !ModelState.IsValid) return View(teacher);

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            var cmd = new MySqlCommand("UPDATE teachers SET teacherfname=@TeacherFName, teacherlname=@TeacherLName, EmployeeNumber=@EmployeeNumber, HireDate=@HireDate, Salary=@Salary WHERE teacherid=@id", conn);
            cmd.Parameters.AddWithValue("@TeacherFName", teacher.TeacherFName);
            cmd.Parameters.AddWithValue("@TeacherLName", teacher.TeacherLName);
            cmd.Parameters.AddWithValue("@EmployeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@Salary", teacher.Salary);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0) return NotFound();

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
                    TeacherFName = reader.GetString("teacherfname"),
                    TeacherLName = reader.GetString("teacherlname"),
                    EmployeeNumber = reader.GetString("employeenumber"),
                    HireDate = reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : reader.GetDecimal("Salary")

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
    }
}


