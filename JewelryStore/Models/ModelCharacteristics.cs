﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Characteristics")]
    public class ModelCharacteristics
    {
        [Key]
        public int ID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        public List<ModelCharacteristicValues> CharacteristicValues { get; set; }
    }
}
