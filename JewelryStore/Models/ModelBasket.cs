using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Basket")]
    public class ModelBasket
    {
        [Key]
        public int ID { get; set; }

        public int ID_User { get; set; }
        [ForeignKey("ID_User")]
        public ModelUsers User { get; set; }
    }
}
