using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models.DataBase
{
    public class FilterModel
    {
        [JsonPropertyName("jewelry_kinds")]
        public List<JewelryKindsModel> JewelryKinds { get; set; }
        [JsonPropertyName("characteristics")]
        public List<CharacteristicsModel> Characteristics { get; set; }

        public FilterModel(List<JewelryKindsModel> jewelryKinds, List<CharacteristicsModel> characteristics)
        {
            JewelryKinds = jewelryKinds;
            Characteristics = characteristics;
        }
    }
}
