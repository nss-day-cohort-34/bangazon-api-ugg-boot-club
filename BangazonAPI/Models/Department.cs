using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using BangazonAPI.Models;

namespace BangazonAPI.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }
    }
}