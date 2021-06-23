using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace JewelryStore.ViewModels
{
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
}
