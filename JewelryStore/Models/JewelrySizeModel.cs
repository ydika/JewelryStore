using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        public JewelrySizeModel(int iD_Jewelry, string size, string price)
        {
            ID_Jewelry = iD_Jewelry;
            Size = size;
            Price = price;
        }
    }
}
