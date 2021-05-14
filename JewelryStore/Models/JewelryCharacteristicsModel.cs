using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("JewelryCharacteristics")]
    public class JewelryCharacteristicsModel
    {
        [Key]
        public int ID { get; set; }

        public int ID_Jewelry { get; set; }
        [ForeignKey("ID_Jewelry")]
        public JewelryModel Jewelry { get; set; }

        public int ID_Characteristic { get; set; }
        [ForeignKey("ID_Characteristic")]
        public CharacteristicValuesModel CharacteristicValues { get; set; }
    }
}
