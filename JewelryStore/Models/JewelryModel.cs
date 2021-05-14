﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Jewelry")]
    public class JewelryModel
    {
        [Key]
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Kind { get; set; }
        [JsonIgnore]
        [ForeignKey("ID_Kind")]
        public JewelryKindsModel Kind { get; set; }

        [JsonIgnore]
        public int ID_Discount { get; set; }
        [ForeignKey("ID_Discount")]
        public DiscountsModel Discount { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
        [JsonIgnore]
        [StringLength(200)]
        public string InsertedGemChar { get; set; }
        public double Price { get; set; }
        [JsonIgnore]
        [StringLength(30)]
        public string Code { get; set; }
        [JsonIgnore]
        public int Quantity { get; set; }
        [StringLength(100)]
        public string ImageSrc { get; set; }
        [StringLength(200)]
        public string Url { get; set; }

        [JsonIgnore]
        public List<JewelryCharacteristicsModel> JewelryCharacteristics { get; set; }
    }
}
