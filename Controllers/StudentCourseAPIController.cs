using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCourseAPIController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentCourseAPIController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
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
            return Ok(list);
        }

        // GET BY ID
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            StudentCourse sc = null;
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM studentsxcourses WHERE studentxcoursid=@id", conn);
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
            return sc == null ? NotFound("StudentCourse not found.") : Ok(sc);
        }

        // CREATE
        [HttpPost]
        public IActionResult Create([FromBody] StudentCourse sc)
        {
            if (sc.StudentId <= 0 || sc.CourseId <= 0)
                return BadRequest("StudentId and CourseId are required.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "INSERT INTO studentsxcourses (studentid, courseid) VALUES (@studentid, @courseid)", conn);
            cmd.Parameters.AddWithValue("@studentid", sc.StudentId);
            cmd.Parameters.AddWithValue("@courseid", sc.CourseId);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("StudentCourse added.") : StatusCode(500, "Error adding StudentCourse.");
        }

        // UPDATE
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] StudentCourse sc)
        {
            if (sc.StudentId <= 0 || sc.CourseId <= 0)
                return BadRequest("StudentId and CourseId are required.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Check existence
            using var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM studentsxcourses WHERE studentxcoursid=@id", conn);
            checkCmd.Parameters.AddWithValue("@id", id);
            var count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count == 0) return NotFound("StudentCourse not found.");

            using var cmd = new MySqlCommand(
                "UPDATE studentsxcourses SET studentid=@studentid, courseid=@courseid WHERE studentxcoursid=@id", conn);
            cmd.Parameters.AddWithValue("@studentid", sc.StudentId);
            cmd.Parameters.AddWithValue("@courseid", sc.CourseId);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("StudentCourse updated.") : StatusCode(500, "Error updating StudentCourse.");
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM studentsxcourses WHERE studentxcoursid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("StudentCourse deleted.") : NotFound("StudentCourse not found.");
        }

        // GET STUDENTS (for dropdown)
        [HttpGet("Students")]
        public IActionResult GetStudents()
        {
            var list = new List<object>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT studentid, studentfname, studentlname FROM students", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new
                {
                    StudentId = reader.GetInt32("studentid"),
                    Name = $"{reader.GetString("studentfname")} {reader.GetString("studentlname")}"
                });
            }
            return Ok(list);
        }

        // GET COURSES (for dropdown)
        [HttpGet("Courses")]
        public IActionResult GetCourses()
        {
            var list = new List<object>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand("SELECT courseid, coursecode, coursename FROM courses", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new
                {
                    CourseId = reader.GetInt32("courseid"),
                    Name = $"{reader.GetString("coursecode")} - {reader.GetString("coursename")}"
                });
            }
            return Ok(list);
        }
    }
}
