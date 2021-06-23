using JewelryStore.Models;
using System.Collections.Generic;

namespace JewelryStore.ViewModels
{
    public class JewelrySliderViewModel
    {
        public List<JewelryModel> JewelriesWithDiscount { get; set; }
        public List<JewelryModel> NewJewelries { get; set; }

        public JewelrySliderViewModel(List<JewelryModel> jewelriesWithDiscount, List<JewelryModel> newJewelries)
        {
            JewelriesWithDiscount = jewelriesWithDiscount;
            NewJewelries = newJewelries;
        }
    }
}
