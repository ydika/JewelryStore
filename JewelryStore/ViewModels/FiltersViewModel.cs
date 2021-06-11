using JewelryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class FiltersViewModel
    {
        public List<JewelryKindsModel> JewelryKinds { get; set; }
        public List<CharacteristicsModel> Characteristics { get; set; }

        public string MaxPrice { get; set; }
        public string MinPrice { get; set; }

        public FiltersViewModel(List<JewelryKindsModel> jewelryKinds, List<CharacteristicsModel> characteristics, string maxPrice, string minPrice)
        {
            JewelryKinds = jewelryKinds;
            Characteristics = characteristics;
            MaxPrice = maxPrice;
            MinPrice = minPrice;
        }
    }
}
