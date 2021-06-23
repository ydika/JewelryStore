using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JewelryStore.Models
{
    [Table("Characteristics")]
    public class CharacteristicsModel
    {
        [Key]
        public int ID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        [JsonPropertyName("characteristic_values")]
        public List<CharacteristicValueModel> CharacteristicValues { get; set; }
    }
}
