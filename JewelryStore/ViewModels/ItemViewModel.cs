﻿using JewelryStore.Models;
using System.Collections.Generic;

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
