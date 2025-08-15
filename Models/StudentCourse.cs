using System.ComponentModel.DataAnnotations;

namespace AnkitKumar_SchoolApp.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentXCourseId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Student")]
        public int? StudentId { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Course")]
        public int? CourseId { get; set; }

    }
}
