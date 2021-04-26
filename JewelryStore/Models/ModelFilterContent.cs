using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    public class ModelFilterContent
    {
        public string CharacteristicName { get; set; }
        public List<ModelCharacteristicValues> CharacteristicValues { get; set; }

        public ModelFilterContent(string characteristicNames, List<ModelCharacteristicValues> characteristicValues)
        {
            CharacteristicName = characteristicNames;
            CharacteristicValues = characteristicValues;
        }
    }
}
