using JewelryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class PurchaseViewModel
    {
        public string PurchaseCode { get; set; }
        public CartModel Cart { get; set; }

        public PurchaseViewModel(string purchaseCode, CartModel cart)
        {
            PurchaseCode = purchaseCode;
            Cart = cart;
        }
    }
}
