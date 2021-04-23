using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Jewelry")]
    public class ModelJewelry
    {
        [Key]
        public int ID { get; set; }

        public int ID_Kind { get; set; }
        [ForeignKey("ID_Kind")]
        public ModelJewelryKinds Kind { get; set; }

        public int ID_Discount { get; set; }
        [ForeignKey("ID_Discount")]
        public ModelDiscounts Discount { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(200)]
        public string InsertedGemChar { get; set; }
        public double Price { get; set; }
        [StringLength(30)]
        public string Code { get; set; }
        public int Quantity { get; set; }
        [StringLength(100)]
        public string ImageSrc { get; set; }
    }
}
