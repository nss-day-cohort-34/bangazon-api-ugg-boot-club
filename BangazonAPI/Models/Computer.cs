using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using BangazonAPI.Models;

namespace BangazonAPI.Models
{
    public class Computer
    {
        public int Id { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }
        public DateTime DecommissionDate { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        public int CurrentEmployeeId { get; set; }
    }
}