using AnkitKumar_SchoolApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace AnkitKumar_SchoolApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseAPIController : ControllerBase
    {
        private readonly string _connectionString;

        public CourseAPIController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SchoolDbConnection");
        }

        // GET: api/CourseAPI
        [HttpGet]
        public IActionResult GetAllCourses()
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

            return Ok(courses);
        }

        // GET: api/CourseAPI/5
        [HttpGet("{id:int}")]
        public IActionResult GetCourseById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Course ID.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM courses WHERE courseid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var course = new Course
                {
                    CourseId = reader.GetInt32("courseid"),
                    CourseCode = reader.IsDBNull(reader.GetOrdinal("coursecode")) ? null : reader.GetString("coursecode"),
                    TeacherId = reader.IsDBNull(reader.GetOrdinal("teacherid")) ? null : reader.GetInt64("teacherid"),
                    StartDate = reader.IsDBNull(reader.GetOrdinal("startdate")) ? (DateTime?)null : reader.GetDateTime("startdate"),
                    FinishDate = reader.IsDBNull(reader.GetOrdinal("finishdate")) ? (DateTime?)null : reader.GetDateTime("finishdate"),
                    CourseName = reader.IsDBNull(reader.GetOrdinal("coursename")) ? null : reader.GetString("coursename")
                };
                return Ok(course);
            }

            return NotFound("Course not found.");
        }

        // POST: api/CourseAPI
        [HttpPost]
        public IActionResult AddCourse([FromBody] Course course)
        {
            var validation = ValidateCourse(course);
            if (validation != null) return validation;

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
            return Ok("Course added.");
        }

        // PUT: api/CourseAPI/5
        [HttpPut("{id:int}")]
        public IActionResult UpdateCourse(int id, [FromBody] Course course)
        {
            if (id <= 0) return BadRequest("Invalid Course ID.");

            var validation = ValidateCourse(course);
            if (validation != null) return validation;

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            // Check existence
            var existsCmd = new MySqlCommand("SELECT COUNT(*) FROM courses WHERE courseid=@id", conn);
            existsCmd.Parameters.AddWithValue("@id", id);
            if (Convert.ToInt32(existsCmd.ExecuteScalar()) == 0) return NotFound("Course not found.");

            var cmd = new MySqlCommand(
                "UPDATE courses SET coursecode=@code, teacherid=@teacherId, startdate=@start, finishdate=@finish, coursename=@name WHERE courseid=@id", conn);

            cmd.Parameters.AddWithValue("@code", course.CourseCode);
            cmd.Parameters.AddWithValue("@teacherId", course.TeacherId.HasValue ? (object)course.TeacherId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@start", course.StartDate.HasValue ? (object)course.StartDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@finish", course.FinishDate.HasValue ? (object)course.FinishDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@name", course.CourseName);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
            return Ok("Course updated.");
        }

        // DELETE: api/CourseAPI/5
        [HttpDelete("{id:int}")]
        public IActionResult DeleteCourse(int id)
        {
            if (id <= 0) return BadRequest("Invalid Course ID.");

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("DELETE FROM courses WHERE courseid=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? Ok("Course deleted.") : NotFound("Course not found.");
        }

        private IActionResult? ValidateCourse(Course course)
        {
            if (course == null) return BadRequest("Course payload is required.");
            if (string.IsNullOrWhiteSpace(course.CourseCode)) return BadRequest("Course code is required.");
            if (string.IsNullOrWhiteSpace(course.CourseName)) return BadRequest("Course name is required.");
            if (course.TeacherId.HasValue && course.TeacherId.Value <= 0) return BadRequest("TeacherId must be greater than 0.");

            if (course.StartDate.HasValue && course.FinishDate.HasValue && course.StartDate.Value.Date > course.FinishDate.Value.Date)
                return BadRequest("StartDate cannot be after FinishDate.");

            return null;
        }
    }
}
