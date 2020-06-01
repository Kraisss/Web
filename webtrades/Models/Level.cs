//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace webtrades.Models
//{
//    public class Level
//    {
//        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }

//        [Required]
//        [Range(0, 100, ErrorMessage = "{0} должен быть не менее {1} и не более {2}")]
//        public int RestrictionOnBuy { get; set; }

//    }
//}
