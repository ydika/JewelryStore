using JewelryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JewelryStore.ViewModels
{
    public class CardsViewModel
    {
        public List<JewelryModel> Jewelries { get; set; }
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }
        [JsonPropertyName("page_count")]
        public int PageCount { get; set; }

        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
        public string SearchName { get; set; }

        public CardsViewModel(List<JewelryModel> jewelries, int currentPage, int pageCount)
        {
            Jewelries = jewelries;
            CurrentPage = currentPage;
            PageCount = pageCount;
        }

        public CardsViewModel(List<JewelryModel> jewelries, int currentPage, int pageCount, double maxPrice, double minPrice, string searchName) : this(jewelries, currentPage, pageCount)
        {
            MaxPrice = maxPrice;
            MinPrice = minPrice;
            SearchName = searchName;
        }
    }
}
