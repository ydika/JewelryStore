using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Discounts")]
    public class DiscountsModel
    {
        [Key]
        public int ID { get; set; }
        public double Amount { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
    }
}
