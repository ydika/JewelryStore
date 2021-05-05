using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    public class ModelDynamicFilter
    {
        public string CharacteristicName { get; set; }
        public List<string> CharacteristicValues { get; set; }

        public ModelDynamicFilter(string characteristicNames, List<string> characteristicValues)
        {
            CharacteristicName = characteristicNames;
            CharacteristicValues = characteristicValues;
        }
    }
}
