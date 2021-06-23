using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace JewelryStore.Models
{
    [Table("Jewelry")]
    public class JewelryModel
    {
        [Key]
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Subspecies { get; set; }
        [JsonIgnore]
        [ForeignKey("ID_Subspecies")]
        public SubspeciesModel Subspecies { get; set; }

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

        private string price;
        [StringLength(10)]
        [Required(ErrorMessage = "Не указана цена")]
        [RegularExpression(@"^\d+\.\d{2}$", ErrorMessage = "Цена должна быть вида [0.00]")]
        public string Price
        {
            get
            {
                if (Discount != null)
                {
                    return Math.Round(double.Parse(price, CultureInfo.InvariantCulture) * (1 - double.Parse(Discount.Amount.ToString()) / 100), 2).ToString("0.00");
                }
                return price;
            }
            set => price = value;
        }

        [JsonIgnore]
        [StringLength(30)]
        [Required(ErrorMessage = "Не указан код/артикль")]
        public string Code { get; set; }

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

        public List<JewelrySizeModel> JewelrySizes { get; set; }
    }
}
