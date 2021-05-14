using JewelryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class ItemViewModel
    {
        public JewelryModel Jewelry { get; set; }
        public List<ItemCharacteristicsViewModel> Characteristics { get; set; }

        public ItemViewModel(JewelryModel jewelry, List<ItemCharacteristicsViewModel> characteristics)
        {
            Jewelry = jewelry;
            Characteristics = characteristics;
        }
    }
}
