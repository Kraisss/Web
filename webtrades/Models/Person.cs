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
    public class Person
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Модель пользователя из таблицы People
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [NotMapped]
        public string Password { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        [Required]
        public double PersonalAccount { get; set; }

        [ForeignKey("role")]
        [Required]
        public int RoleId { get; set; }
        public Role role { get; set; }

        [Required]
        [Range(0, 4, ErrorMessage = "{0} должен быть не менее {1} и не более {2}")]
        //[ForeignKey("level")]
        public int Level { get; set; }
       // public Level level { get; set; }

        public ICollection<ItemPersonAccount> Accounts { get; set; }//Связь один ко многим с таблицей ItemPersonAccounts
        public ICollection<TradeOperation> Operations { get; set; }//Связь один ко многим с таблицей TradeOperation
        public Person()
        {
           Operations = new List<TradeOperation>();
            Accounts = new List<ItemPersonAccount>();
        }
    }
}