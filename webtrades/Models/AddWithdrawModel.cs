using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class AddWithdrawModel
    {
        public string Amount { get; set; }//Сумма средства
        public string Action { get; set; }//Тип действия
    }
}
