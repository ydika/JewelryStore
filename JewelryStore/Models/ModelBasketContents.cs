using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("BasketContents")]
    public class ModelBasketContents
    {
        [Key]
        public int ID { get; set; }

        public int ID_Jewelry { get; set; }
        [ForeignKey("ID_Jewelry")]
        public ModelJewelry Jewelry { get; set; }

        public int ID_Basket { get; set; }
        [ForeignKey("ID_Basket")]
        public ModelBasket Basket { get; set; }

        public DateTime DateOfPlacement { get; set; }
        public int Quantity { get; set; }
    }
}
