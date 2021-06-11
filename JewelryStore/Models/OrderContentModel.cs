using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("OrderContent")]
    public class OrderContentModel
    {
        [Key]
        public int ID { get; set; }

        public int ID_Jewelry { get; set; }
        [ForeignKey("ID_Jewelry")]
        public JewelryModel Jewelry { get; set; }

        public int ID_Order { get; set; }
        [ForeignKey("ID_Order")]
        public OrdersModel Order { get; set; }

        public DateTime DateOfPlacement { get; set; }
        public int Quantity { get; set; }
        [StringLength(10)]
        public string TotalPrice { get; set; }
        [StringLength(10)]
        public string Size { get; set; }
    }
}
