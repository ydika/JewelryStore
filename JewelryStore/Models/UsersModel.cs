using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("Users")]
    public class UsersModel
    {
        [Key]
        public int ID { get; set; }
        [StringLength(100)]
        public string Login { get; set; }
        [StringLength(40)]
        public string Password { get; set; }
    }
}
