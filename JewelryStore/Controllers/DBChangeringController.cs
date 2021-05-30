using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DBChangeringController : Controller
    {
        public IActionResult DataBaseEditor()
        {
            return View();
        }
    }
}
