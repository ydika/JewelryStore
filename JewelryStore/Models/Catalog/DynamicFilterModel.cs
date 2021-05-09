using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models.DataBase
{
    public class DynamicFilterModel
    {
        public string CharacteristicName { get; set; }
        public List<string> CharacteristicValues { get; set; }

        public DynamicFilterModel(string characteristicNames, List<string> characteristicValues)
        {
            CharacteristicName = characteristicNames;
            CharacteristicValues = characteristicValues;
        }
    }
}
