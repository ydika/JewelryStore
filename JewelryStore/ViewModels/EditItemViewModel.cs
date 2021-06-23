using JewelryStore.Models;
using System.Collections.Generic;

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
