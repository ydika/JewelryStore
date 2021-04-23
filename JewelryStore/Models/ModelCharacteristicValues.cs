using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("CharacteristicValues")]
    public class ModelCharacteristicValues
    {
        [Key]
        public int ID { get; set; }

        public int ID_Characteristic { get; set; }
        [ForeignKey("ID_Characteristic")]
        public ModelCharacteristics Characteristic { get; set; }

        [StringLength(100)]
        public string Value { get; set; }
    }
}
