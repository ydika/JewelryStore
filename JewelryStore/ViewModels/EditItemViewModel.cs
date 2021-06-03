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

    public class SelectedItemCharacteristics
    {
        public string Name { get; set; }
        public List<string> SelectedValue { get; set; }
        public List<SelectListItem> Values { get; set; }

        public SelectedItemCharacteristics(string name, List<string> selectedValue, List<SelectListItem> values)
        {
            Name = name;
            SelectedValue = selectedValue;
            Values = values;
        }
    }

    public class SelectedKind
    {
        public string Kind { get; set; }
        public List<SelectListItem> Kinds { get; set; }

        public SelectedKind(string kind, List<SelectListItem> kinds)
        {
            Kind = kind;
            Kinds = kinds;
        }
    }

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
