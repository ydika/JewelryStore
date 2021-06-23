using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Models
{
    [Table("Discounts")]
    public class DiscountModel
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Размер скидки не указан")]
        [Range(0, 100, ErrorMessage = "Скидка может быть в диапазоне от 0 до 100")]
        public int Amount { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
    }
}
