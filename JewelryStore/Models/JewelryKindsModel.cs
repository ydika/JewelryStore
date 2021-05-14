using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("JewelryKinds")]
    public class JewelryKindsModel
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string RuName { get; set; }
        [StringLength(50)]
        public string EnName { get; set; }
        [StringLength(100)]
        public string IconSrc { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
    }
}
