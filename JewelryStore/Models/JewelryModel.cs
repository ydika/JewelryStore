using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Jewelry")]
    public class JewelryModel
    {
        [Key]
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Kind { get; set; }
        [JsonIgnore]
        [ForeignKey("ID_Kind")]
        public JewelryKindsModel Kind { get; set; }

        [JsonIgnore]
        public int ID_Discount { get; set; }
        [ForeignKey("ID_Discount")]
        public DiscountModel Discount { get; set; }

        [StringLength(100, ErrorMessage = "Максимальная длина - 100 символов")]
        [MinLength(5, ErrorMessage = "Минимальная длина названия - 5 символов")]
        [Required(ErrorMessage = "Не указано название")]
        public string Name { get; set; }

        [JsonIgnore]
        [StringLength(200)]
        public string InsertedGemChar { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "Не указана цена")]
        [RegularExpression(@"^\d+\.\d{2}$", ErrorMessage = "Цена должна быть вида [0.00]")]
        public string Price { get; set; }

        [JsonIgnore]
        [StringLength(30)]
        [Required(ErrorMessage = "Не указан код/артикль")]
        public string Code { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "Не указано количество")]
        public int Quantity { get; set; }

        [StringLength(100)]
        public string ImageSrc { get; set; } = "/images/jewelrys/noimage.png";

        [StringLength(200)]
        public string Url { get; set; }

        [JsonIgnore]
        public List<JewelryCharacteristicsModel> JewelryCharacteristics { get; set; }
        [JsonIgnore]
        public List<CartContentModel> CartContents { get; set; }
    }
}
