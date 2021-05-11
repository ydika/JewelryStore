using JewelryStore.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.Models.Catalog
{
    public class CardsModel
    {
        public List<JewelryModel> Jewelries { get; set; }
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
        [JsonPropertyName("page_count")]
        public int PageCount { get; set; }

        public CardsModel(List<JewelryModel> jewelries, int currentPage, int pageCount)
        {
            Jewelries = jewelries;
            CurrentPage = currentPage;
            PageCount = pageCount;
        }
    }
}
