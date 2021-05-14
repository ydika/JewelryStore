using JewelryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class ItemCharacteristicsViewModel
    {
        public string Name { get; set; }
        public List<CharacteristicValuesModel> Value { get; set; }

        public ItemCharacteristicsViewModel(string name, List<CharacteristicValuesModel> value)
        {
            Name = name;
            Value = value;
        }
    }
}
