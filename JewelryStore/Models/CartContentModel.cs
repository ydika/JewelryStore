using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("CartContent")]
    public class CartContentModel
    {
        [Key]
        public int ID { get; set; }

        [JsonIgnore]
        public int ID_Cart{ get; set; }
        [JsonIgnore]
        [ForeignKey("ID_Cart")]
        public CartModel Cart { get; set; }

        [JsonIgnore]
        public int ID_Jewelry { get; set; }
        [ForeignKey("ID_Jewelry")]
        public JewelryModel Jewelry { get; set; }

        public DateTime DateOfPlacement { get; set; }
        public int Quantity { get; set; }
        [JsonPropertyName("total_price")]
        public string TotalPrice { get; set; }

        public CartContentModel(int iD_Cart, int iD_Jewelry, DateTime dateOfPlacement, int quantity, string totalPrice)
        {
            ID_Cart = iD_Cart;
            ID_Jewelry = iD_Jewelry;
            DateOfPlacement = dateOfPlacement;
            Quantity = quantity;
            TotalPrice = totalPrice;
        }
    }
}
