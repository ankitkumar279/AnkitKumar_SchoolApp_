using System;
using System.ComponentModel.DataAnnotations;

namespace AnkitKumar_SchoolApp.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string StudentFName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string StudentLName { get; set; }

        [Required(ErrorMessage = "Student Number is required")]
        public string StudentNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EnrolDate { get; set; }
    }
}
