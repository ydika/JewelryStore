using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    public class ModelCards
    {
        public List<ModelJewelry> Jewelries { get; set; }
        public int JewelriesCount { get; set; }

        public ModelCards(List<ModelJewelry> jewelries, int jewelriesCount)
        {
            Jewelries = jewelries;
            JewelriesCount = jewelriesCount;
        }
    }
}
