using JewelryStore.Models;
using JewelryStore.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class CatalogViewModel
    {
        public List<JewelryModel> Jewelries { get; set; }
        public List<JewelryKindsModel> JewelryKinds { get; set; }
        public List<DynamicFilterModel> Filter { get; set; }

        public CatalogViewModel(List<JewelryModel> jewelries, List<JewelryKindsModel> jewelryKinds, 
            List<DynamicFilterModel> filter)
        {
            Jewelries = jewelries;
            JewelryKinds = jewelryKinds;
            Filter = filter;
        }
    }
}
