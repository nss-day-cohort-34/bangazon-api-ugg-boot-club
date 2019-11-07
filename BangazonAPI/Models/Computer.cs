<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
=======
﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using BangazonAPI.Models;
>>>>>>> master

namespace BangazonAPI.Models
{
    public class Computer
    {
        public int Id { get; set; }
<<<<<<< HEAD
        public DateTime PurchaseDate { get; set; }
        public DateTime DecomissionDate { get; set; }
        public string Make { get; set; }
        public string Manufacturer { get; set; }
        public int CurrentEmployeeId {get; set;}
    }
}
=======

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
>>>>>>> master
