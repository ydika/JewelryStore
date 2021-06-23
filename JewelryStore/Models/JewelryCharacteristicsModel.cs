using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int ID_CharacteristicValue { get; set; }
        [ForeignKey("ID_CharacteristicValue")]
        public CharacteristicValueModel CharacteristicValues { get; set; }
    }
}
