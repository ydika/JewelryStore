using JewelryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class CatalogViewModel
    {
        public List<ModelJewelry> Jewelries { get; set; }
        public List<ModelJewelryKinds> JewelryKinds { get; set; }
        public List<ModelFilterContent> Filter { get; set; }

        public CatalogViewModel(List<ModelJewelry> jewelries, List<ModelJewelryKinds> jewelryKinds, 
            List<ModelFilterContent> filter)
        {
            Jewelries = jewelries;
            JewelryKinds = jewelryKinds;
            Filter = filter;
        }
    }
}
