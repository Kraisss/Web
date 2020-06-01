using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class HomeModel
    {
         public Person person { get; set; }//Авторизированный пользователь
        public Item itemchoosed { get; set; }//Выбранный товар
        public List<Item> items { get; set; }//Список всех товаров
        public ItemPersonAccount personAccount { get; set; }//Личный баланс товара авторизированного пользователя
        public string Amount { get; set; }//Количество товара для покупки/продажи
        public List<ExchangeRateHistory> History { get; set; }//История изменения курса
    }
}
