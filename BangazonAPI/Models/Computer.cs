
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using BangazonAPI.Models;


namespace BangazonAPI.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime DecomissionDate { get; set; }
        public string Make { get; set; }
        public string Manufacturer { get; set; }
        public int CurrentEmployeeId {get; set;}
    }
}

