using System;
using System.ComponentModel.DataAnnotations;

namespace AnkitKumar_SchoolApp.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course Code is required")]
        public string CourseCode { get; set; }

        public long? TeacherId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FinishDate { get; set; }

        [Required(ErrorMessage = "Course Name is required")]
        public string CourseName { get; set; }
    }
}
