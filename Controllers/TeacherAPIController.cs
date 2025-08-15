using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly string _connectionString;

        public TeacherAPIController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        /// <summary>
        /// Returns all teachers.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Teacher>), 200)]
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
                    TeacherFName = reader.IsDBNull(reader.GetOrdinal("teacherfname")) ? null : reader.GetString("teacherfname"),
                    TeacherLName = reader.IsDBNull(reader.GetOrdinal("teacherlname")) ? null : reader.GetString("teacherlname"),
                    EmployeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) ? null : reader.GetString("employeenumber"),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate")) ? (DateTime?)null : reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? (decimal?)null : reader.GetDecimal("salary")
                });
            }

            return Ok(teachers);
        }

        /// <summary>
        /// Returns a single teacher by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Teacher), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetTeacherById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

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
                    TeacherFName = reader.IsDBNull(reader.GetOrdinal("teacherfname")) ? null : reader.GetString("teacherfname"),
                    TeacherLName = reader.IsDBNull(reader.GetOrdinal("teacherlname")) ? null : reader.GetString("teacherlname"),
                    EmployeeNumber = reader.IsDBNull(reader.GetOrdinal("employeenumber")) ? null : reader.GetString("employeenumber"),
                    HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate")) ? (DateTime?)null : reader.GetDateTime("hiredate"),
                    Salary = reader.IsDBNull(reader.GetOrdinal("salary")) ? (decimal?)null : reader.GetDecimal("salary")
                };

                return Ok(teacher);
            }

            return NotFound("Teacher not found.");
        }

        /// <summary>
        /// Adds a new teacher.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult AddTeacher([FromBody] Teacher teacher)
        {
            var validation = ValidateTeacherForUpdateOrCreate(teacher, isCreate: true);
            if (validation is not null) return validation;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand(
                "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) " +
                "VALUES (@TeacherFName, @TeacherLName, @EmployeeNumber, @HireDate, @Salary)", conn);

            cmd.Parameters.AddWithValue("@TeacherFName", (object?)teacher.TeacherFName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TeacherLName", (object?)teacher.TeacherLName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EmployeeNumber", (object?)teacher.EmployeeNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HireDate", (object?)teacher.HireDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Salary", (object?)teacher.Salary ?? DBNull.Value);

            var result = cmd.ExecuteNonQuery();
            return result > 0 ? Ok("Teacher added.") : StatusCode(500, "Error adding teacher.");
        }

        /// <summary>
        /// Updates an existing teacher by id.
        /// </summary>
        /// <param name="id">Teacher ID to update.</param>
        /// <param name="teacher">Updated teacher payload.</param>
        [HttpPut("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher teacher)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            // Server-side business validations (Initiative marks)
            var validation = ValidateTeacherForUpdateOrCreate(teacher, isCreate: false);
            if (validation is not null) return validation;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Ensure the teacher exists
            using (var existsCmd = new MySqlCommand("SELECT COUNT(*) FROM teachers WHERE teacherid=@id", conn))
            {
                existsCmd.Parameters.AddWithValue("@id", id);
                var count = Convert.ToInt32(existsCmd.ExecuteScalar());
                if (count == 0) return NotFound("Teacher not found.");
            }

            // Perform update
            var updateCmd = new MySqlCommand(
                @"UPDATE teachers 
                  SET teacherfname=@TeacherFName,
                      teacherlname=@TeacherLName,
                      employeenumber=@EmployeeNumber,
                      hiredate=@HireDate,
                      salary=@Salary
                  WHERE teacherid=@id", conn);

            updateCmd.Parameters.AddWithValue("@TeacherFName", (object?)teacher.TeacherFName ?? DBNull.Value);
            updateCmd.Parameters.AddWithValue("@TeacherLName", (object?)teacher.TeacherLName ?? DBNull.Value);
            updateCmd.Parameters.AddWithValue("@EmployeeNumber", (object?)teacher.EmployeeNumber ?? DBNull.Value);
            updateCmd.Parameters.AddWithValue("@HireDate", (object?)teacher.HireDate ?? DBNull.Value);
            updateCmd.Parameters.AddWithValue("@Salary", (object?)teacher.Salary ?? DBNull.Value);
            updateCmd.Parameters.AddWithValue("@id", id);

            var rows = updateCmd.ExecuteNonQuery();
            if (rows == 0) return NotFound("Teacher not found.");

            return Ok("Teacher updated.");
        }

        /// <summary>
        /// Deletes a teacher by id.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult DeleteTeacher(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM teachers WHERE teacherid = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            var result = cmd.ExecuteNonQuery();
            return result > 0 ? Ok("Teacher deleted.") : NotFound("Teacher not found.");
        }

        /// <summary>
        /// Central server-side validation for Teacher create/update.
        /// (Initiative marks: empty name, future hire date, salary &lt; 0)
        /// </summary>
        private IActionResult? ValidateTeacherForUpdateOrCreate(Teacher teacher, bool isCreate)
        {
            if (teacher is null) return BadRequest("Teacher payload is required.");

            // Name required
            if (string.IsNullOrWhiteSpace(teacher.TeacherFName) || string.IsNullOrWhiteSpace(teacher.TeacherLName))
                return BadRequest("First and Last name are required.");

            // Future hire date not allowed
            if (teacher.HireDate.HasValue && teacher.HireDate.Value.Date > DateTime.UtcNow.Date)
                return BadRequest("Hire date cannot be in the future.");

            // Salary must be >= 0
            if (teacher.Salary.HasValue && teacher.Salary.Value < 0)
                return BadRequest("Salary must be 0 or greater.");

            return null;
        }
    }
}
