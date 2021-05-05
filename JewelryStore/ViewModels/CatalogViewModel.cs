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
        public List<ModelDynamicFilter> Filter { get; set; }

        public CatalogViewModel(List<ModelJewelry> jewelries, List<ModelJewelryKinds> jewelryKinds, 
            List<ModelDynamicFilter> filter)
        {
            Jewelries = jewelries;
            JewelryKinds = jewelryKinds;
            Filter = filter;
        }
    }
}
