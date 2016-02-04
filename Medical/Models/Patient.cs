using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Medical.Models
{
    public class Patient
    {
        public int ID { get; set; }

        [Display(Name = "Patient")]
        public string FullName
        {
            get
            {
                return FirstName + (string.IsNullOrEmpty(MiddleName) ? " " :
                    (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        [Required(ErrorMessage = "You cannot leave OHIP number blank")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "The OHIP number must be exactly 10 numbers")]
        [StringLength(10)]
        public string OHIP { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [Required(ErrorMessage = "Middle name is required")]
        [StringLength(50)]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd}", ApplyFormatInEditMode=true)]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Display(Name = "Visits/Yr")]
        [Required(ErrorMessage = "You cannot leave the number of expected visits per year blank.")]
        [Range(1, 12, ErrorMessage = "The number of expected visits per year must be between 1 and 12.")]
        public byte ExpYrVisits { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; } // Added for Concurrency

        [Required(ErrorMessage = "You must select a primary care physician.")]
        public int DoctorID { get; set; }

        public virtual Doctor Doctor { get; set; }
    }
}