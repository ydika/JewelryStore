using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("JewelrySize")]
    public class JewelrySizeModel
    {
        [Key]
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Jewelry { get; set; }
        [JsonIgnore]
        [ForeignKey("ID_Jewelry")]
        public JewelryModel Jewelry { get; set; }

        [StringLength(10)]
        public string Size { get; set; }
        [StringLength(10)]
        public string Price { get; set; }
    }
}
