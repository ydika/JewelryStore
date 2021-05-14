using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("CharacteristicValues")]
    public class CharacteristicValuesModel
    {
        [Key]
        [JsonIgnore]
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Characteristic { get; set; }
        [JsonIgnore]
        [ForeignKey("ID_Characteristic")]
        public CharacteristicsModel Characteristic { get; set; }

        [StringLength(100)]
        public string Value { get; set; }
    }
}
