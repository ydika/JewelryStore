using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JewelryStore.Models
{
    [Table("JewelryKinds")]
    public class JewelryKindsModel
    {
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Не указано русское название")]
        [RegularExpression(@"[\WА-я]*", ErrorMessage = "В этом поле могут быть только русские буквы с разделителями")]
        public string RuName { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Не указано английское название")]
        [RegularExpression(@"[\s\-A-z]*", ErrorMessage = "В этом поле могут быть только английские буквы")]
        public string EnName { get; set; }
        [StringLength(200)]
        [Required(ErrorMessage = "Не указано описание")]
        public string Description { get; set; }

        public List<SubspeciesModel> Subspecies { get; set; }
    }
}
