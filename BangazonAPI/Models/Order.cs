﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
        public string Status { get; set; }
        public bool isCompleted
        {
            get
            {
                if (Status.ToLower() == "complete")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
            

