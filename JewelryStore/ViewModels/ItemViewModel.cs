using JewelryStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class ItemViewModel
    {
        public JewelryModel Jewelry { get; set; }
        public List<ItemCharacteristics> Characteristics { get; set; }

        public ItemViewModel(JewelryModel jewelry, List<ItemCharacteristics> characteristics)
        {
            Jewelry = jewelry;
            Characteristics = characteristics;
        }
    }

    public class ItemCharacteristics
    {
        public string Name { get; set; }
        public List<CharacteristicValueModel> Values { get; set; }

        public ItemCharacteristics(string name, List<CharacteristicValueModel> values)
        {
            Name = name;
            Values = values;
        }
    }
}
