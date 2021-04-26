using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("JewelryCharacteristics")]
    public class ModelJewelryCharacteristics
    {
        [Key]
        public int ID { get; set; }

        public int ID_Jewelry { get; set; }
        [ForeignKey("ID_Jewelry")]
        public ModelJewelry Jewelry { get; set; }

        public int ID_CharacteristicValue { get; set; }
        [ForeignKey("ID_CharacteristicValue")]
        public ModelCharacteristicValues CharacteristicValues { get; set; }
    }
}
