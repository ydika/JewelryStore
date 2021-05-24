using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        [JsonIgnore]
        public int ID_Jewelry { get; set; }
        [ForeignKey("ID_Jewelry")]
        public JewelryModel Jewelry { get; set; }

        public DateTime DateOfPlacement { get; set; }
        public int Quantity { get; set; }
        [JsonPropertyName("total_price")]
        public double TotalPrice { get; set; }

        public CartModel(string iD_User, int iD_Jewelry, DateTime dateOfPlacement, int quantity, double totalPrice)
        {
            ID_User = iD_User;
            ID_Jewelry = iD_Jewelry;
            DateOfPlacement = dateOfPlacement;
            Quantity = quantity;
            TotalPrice = totalPrice;
        }
    }
}
