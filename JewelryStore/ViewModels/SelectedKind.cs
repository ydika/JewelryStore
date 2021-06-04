using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
