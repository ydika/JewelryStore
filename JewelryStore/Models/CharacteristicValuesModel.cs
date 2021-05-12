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
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Characteristic { get; set; }
        [ForeignKey("ID_Characteristic")]
        [JsonIgnore]
        public CharacteristicsModel Characteristic { get; set; }

        [StringLength(100)]
        public string Value { get; set; }
    }
}
