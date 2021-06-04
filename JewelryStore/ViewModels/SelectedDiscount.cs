using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class SelectedDiscount
    {
        public string Discount { get; set; }
        public List<SelectListItem> Discounts { get; set; }

        public SelectedDiscount(string discount, List<SelectListItem> discounts)
        {
            Discount = discount;
            Discounts = discounts;
        }
    }
}
