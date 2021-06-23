using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JewelryStore.Models
{
    public class UserModel : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string SecondName { get; set; }
    }
}
