using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models.DataBase
{
    [Table("Characteristics")]
    public class CharacteristicsModel
    {
        [Key]
        public int ID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        [JsonPropertyName("characteristic_values")]
        public List<CharacteristicValuesModel> CharacteristicValues { get; set; }
    }
}
