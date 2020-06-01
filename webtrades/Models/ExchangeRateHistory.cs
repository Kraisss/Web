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
    public class ExchangeRateHistory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Модель изменения курса товара из таблицы ExchangeRateHistories
        public int Id { get; set; }

        [Required]
        public double ExchangeRateChange { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime DateOfChange {get; set;}

        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public Item Item { get; set; }

    }
}
