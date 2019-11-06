using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace BangazonAPI.Models
{
    public class PaymentType
    {
        public int Id { get; set; }

        [Required]
        public string AcctNumber { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public int CustomerId { get; set; }

    }
}
