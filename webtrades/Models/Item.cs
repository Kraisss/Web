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
    public class Item
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Модель товара из таблицы Items
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double ExchangeRate { get; set; }

        public ICollection<ExchangeRateHistory> RateHistory { get; set; }//Связь один ко многим 
        public Item()
        {
            RateHistory = new List<ExchangeRateHistory>();
        }
    }
}
