using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webtrades.Models
{
    public class ItemPersonAccount
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Модель личного баланса товара из таблицы ItemPersonAccounts
        public int Id { get; set; }

        [Required]
        public double ItemQuantity { get; set; }

        [ForeignKey("Person")]
        [Required]
        public int PersonId { get; set; }
        public Person Person { get; set; }

        [ForeignKey("Item")]
        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }

    }
}
