﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Models
{
    [Table("Files")]
    public class FilesModel
    {
        [Key]
        public int ID { get; set; }
        [StringLength(75)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Path { get; set; }
    }
}
