using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using BangazonAPI.Models;

namespace BangazonAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public bool IsSuperVisor { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Department Department { get; set; }
        public Computer Computer { get; set; }
}
}