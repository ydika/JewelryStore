using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace JewelryStore.Models
{
    [Table("Cart")]
    public class CartModel
    {
        [Key]
        public int ID { get; set; }

        [JsonIgnore]
        public string ID_User { get; set; }
        [JsonIgnore]
        [ForeignKey("ID_User")]
        public UserModel User { get; set; }

        public DateTime DateOfCreation { get; set; }

        public List<CartContentModel> CartContent { get; set; }

        public CartModel(string iD_User, DateTime dateOfCreation)
        {
            ID_User = iD_User;
            DateOfCreation = dateOfCreation;
        }
    }
}
