using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Models
{
    [Table("Orders")]
    public class OrdersModel
    {
        [Key]
        public int ID { get; set; }

        public string ID_User { get; set; }
        [ForeignKey("ID_User")]
        public UserModel User { get; set; }

        public DateTime DateOfPlacement { get; set; }
        [StringLength(10)]
        public string Code { get; set; }

        public List<OrderContentModel> OrderContents { get; set; }
    }
}
