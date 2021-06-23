using JewelryStore.Models;

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
