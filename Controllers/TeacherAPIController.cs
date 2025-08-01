using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        //private readonly SchoolDbContext _context;

        //public TeacherAPIController(SchoolDbContext context)
        //{
        //    _context = context;
        //}

        private readonly string _connectionString;

        public TeacherAPIController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
        //{
        //    return await _context.Teachers.ToListAsync();
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<Teacher>> GetTeacher(int id)
        //{
        //    var teacher = await _context.Teachers.FindAsync(id);

        //    if (teacher == null)
        //    {
        //        return NotFound();
        //    }

        //    return teacher;
        //}

        [HttpGet]
        public IActionResult GetAllTeachers()
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

            return Ok(teachers);
        }

        // Get Teacher by ID
        [HttpGet("{id}")]
        public IActionResult GetTeacherById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var teacher = new Teacher
                {
                    TeacherId = reader.GetInt32("teacherid"),
                    TeacherFName = reader.GetString("teacherfname"),
                    TeacherLName = reader.GetString("teacherlname"),
                    EmployeeNumber = reader.GetString("employeenumber"),
                    HireDate = reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("Salary")) ? 0 : reader.GetDecimal("Salary")

                };

                return Ok(teacher);
            }

            return NotFound("Teacher not found.");
        }

        // Add Teacher
        [HttpPost]
        public IActionResult AddTeacher([FromBody] Teacher teacher)
        {
            if (string.IsNullOrWhiteSpace(teacher.TeacherFName) || string.IsNullOrWhiteSpace(teacher.TeacherLName))
                return BadRequest("First and Last name are required.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@TeacherFName, @TeacherLName, @EmployeeNumber , @HireDate, @Salary)", conn);

            cmd.Parameters.AddWithValue("@TeacherFName", teacher.TeacherFName);
            cmd.Parameters.AddWithValue("@TeacherLName", teacher.TeacherLName);
            cmd.Parameters.AddWithValue("@EmployeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@Salary", teacher.Salary);

            int result = cmd.ExecuteNonQuery();
            return result > 0 ? Ok("Teacher added.") : StatusCode(500, "Error adding teacher.");
        }

        // Update Teacher
        [HttpPut("{id}")]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher teacher)
        {
            if (id <= 0) return BadRequest("Invalid ID.");
            if (string.IsNullOrWhiteSpace(teacher.TeacherFName) || string.IsNullOrWhiteSpace(teacher.TeacherLName))
                return BadRequest("First and Last name are required.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@TeacherFName, @TeacherLName, @EmployeeNumber , @HireDate, @Salary)", conn);

            cmd.Parameters.AddWithValue("@TeacherFName", teacher.TeacherFName);
            cmd.Parameters.AddWithValue("@TeacherLName", teacher.TeacherLName);
            cmd.Parameters.AddWithValue("@EmployeeNumber", teacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", teacher.HireDate);
            cmd.Parameters.AddWithValue("@id", id);

            int result = cmd.ExecuteNonQuery();
            return result > 0 ? Ok("Teacher updated.") : NotFound("Teacher not found.");
        }

        // Delete Teacher
        [HttpDelete("{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            int result = cmd.ExecuteNonQuery();
            return result > 0 ? Ok("Teacher deleted.") : NotFound("Teacher not found.");
        }
    }
}
