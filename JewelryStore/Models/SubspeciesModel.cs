using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Models
{
    [Table("Subspecies")]
    public class SubspeciesModel
    {
        [Key]
        public int ID { get; set; }

        public int ID_Kind { get; set; }
        [ForeignKey("ID_Kind")]
        public JewelryKindsModel Kind { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Не указано русское название")]
        [RegularExpression(@"[\WА-я]*", ErrorMessage = "В этом поле могут быть только русские буквы с разделителями")]
        public string RuName { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Не указано английское название")]
        [RegularExpression(@"[\s\-A-z]*", ErrorMessage = "В этом поле могут быть только английские буквы")]
        public string EnName { get; set; }

        public List<JewelryModel> Jewelries { get; set; }
    }
}
