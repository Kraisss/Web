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
    public class TradeOperation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//Модель торговой операции из TradeOperations
        public int Id { get; set; }

        [Required]
        public double Profit { get; set; }

        [ForeignKey("Item")]
        [Required]
        public int ItemId { get; set; }
        public Item Item { get; set; }

        [ForeignKey("Person")]
        [Required]
        public int PersonId { get; set; }
        public Person Person { get; set; }

        [Required]
        public string OperationType { get; set; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime DateOfOperation { get; set; }

        //[ForeignKey("operationType")]
        //[Required]
        //public int OperationTypeId { get; set; }
        //public OperationType operationType { get; set; }
    }
}
