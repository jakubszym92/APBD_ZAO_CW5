﻿using System;
using System.ComponentModel.DataAnnotations;

namespace APBD_ZAO_CW5.Models
{
    public class ProductWarehouse
    {
        [Required]
        public int IdProduct { get; set; }
        
        [Required]
        public int IdWarehouse { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
