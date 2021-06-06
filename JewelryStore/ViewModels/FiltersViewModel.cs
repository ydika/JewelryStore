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

        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }

        public FiltersViewModel(List<JewelryKindsModel> jewelryKinds, List<CharacteristicsModel> characteristics, double maxPrice, double minPrice)
        {
            JewelryKinds = jewelryKinds;
            Characteristics = characteristics;
            MaxPrice = maxPrice;
            MinPrice = minPrice;
        }
    }
}
