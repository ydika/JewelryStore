using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JewelryStore.Models
{
    [Table("CharacteristicValues")]
    public class CharacteristicValueModel
    {
        [Key]
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
