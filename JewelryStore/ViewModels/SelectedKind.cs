using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace JewelryStore.ViewModels
{
    public class SelectedKind
    {
        public string Kind { get; set; }
        public List<SelectListItem> Kinds { get; set; }

        public SelectedKind(string kind, List<SelectListItem> kinds)
        {
            Kind = kind;
            Kinds = kinds;
        }
    }
}
