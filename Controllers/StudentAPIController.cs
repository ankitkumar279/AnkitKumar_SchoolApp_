using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentAPIController(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        [HttpGet]
        public IActionResult GetAllStudents()
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

            return Ok(students);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetStudentById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM students WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var student = new Student
                {
                    StudentId = reader.GetInt32("studentid"),
                    StudentFName = reader.GetString("studentfname"),
                    StudentLName = reader.GetString("studentlname"),
                    StudentNumber = reader.GetString("studentnumber"),
                    EnrolDate = reader.IsDBNull(reader.GetOrdinal("enroldate"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime("enroldate")
                };
                return Ok(student);
            }

            return NotFound("Student not found");
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            var validation = ValidateStudent(student);
            if (validation != null) return validation;

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
            return Ok("Student added");
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student student)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            var validation = ValidateStudent(student);
            if (validation != null) return validation;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Check if student exists
            var existsCmd = new MySqlCommand("SELECT COUNT(*) FROM students WHERE studentid=@id", conn);
            existsCmd.Parameters.AddWithValue("@id", id);
            if (Convert.ToInt32(existsCmd.ExecuteScalar()) == 0) return NotFound("Student not found");

            var cmd = new MySqlCommand(
                "UPDATE students SET studentfname=@fname, studentlname=@lname, studentnumber=@number, enroldate=@date WHERE studentid=@id", conn);

            cmd.Parameters.AddWithValue("@fname", student.StudentFName);
            cmd.Parameters.AddWithValue("@lname", student.StudentLName);
            cmd.Parameters.AddWithValue("@number", student.StudentNumber);
            cmd.Parameters.AddWithValue("@date", student.EnrolDate.HasValue ? (object)student.EnrolDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            return Ok("Student updated");
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteStudent(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM students WHERE studentid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("Student deleted") : NotFound("Student not found");
        }

        private IActionResult? ValidateStudent(Student student)
        {
            if (student == null) return BadRequest("Student payload required");

            if (string.IsNullOrWhiteSpace(student.StudentFName) || string.IsNullOrWhiteSpace(student.StudentLName))
                return BadRequest("First and Last name required");

            if (student.EnrolDate.HasValue && student.EnrolDate.Value.Date > DateTime.UtcNow.Date)
                return BadRequest("Enrollment date cannot be in the future");

            if (string.IsNullOrWhiteSpace(student.StudentNumber))
                return BadRequest("Student number required");

            return null;
        }
    }
}
