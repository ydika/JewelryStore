using JewelryStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class EditItemViewModel
    {
        public JewelryModel Jewelry { get; set; }
        public List<SelectedItemCharacteristics> Characteristics { get; set; }
        public SelectedKind Kind { get; set; }
        public SelectedDiscount Discount { get; set; }

        public EditItemViewModel(JewelryModel jewelry, List<SelectedItemCharacteristics> characteristics, SelectedKind kind, SelectedDiscount discount)
        {
            Jewelry = jewelry;
            Characteristics = characteristics;
            Kind = kind;
            Discount = discount;
        }
    }
}
