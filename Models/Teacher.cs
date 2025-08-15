using System;
using System.ComponentModel.DataAnnotations;

namespace AnkitKumar_SchoolApp.Models
{
    /// <summary>
    /// Represents a teacher in the School database.
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// Primary key of the teacher (maps to teachers.teacherid).
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// Teacher first name (maps to teachers.teacherfname).
        /// </summary>
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(255)]
        public string TeacherFName { get; set; }

        /// <summary>
        /// Teacher last name (maps to teachers.teacherlname).
        /// </summary>
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(255)]
        public string TeacherLName { get; set; }

        /// <summary>
        /// Employee number (maps to teachers.employeenumber).
        /// </summary>
        [Display(Name = "Employee Number")]
        [Required(ErrorMessage = "Employee number is required.")]
        [StringLength(255)]
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// Hire date (maps to teachers.hiredate).
        /// </summary>
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Hire date is required.")]
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Salary (maps to teachers.salary).
        /// </summary>
        [Display(Name = "Salary")]
        [Required(ErrorMessage = "Salary is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be greater than 0.")]
        public decimal? Salary { get; set; }
    }
}
