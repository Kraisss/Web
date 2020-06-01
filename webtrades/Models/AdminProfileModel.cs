using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class AdminProfileModel
    {
        public IEnumerable<Person> Users { get; set; }//Список пользователей (Admin)
        public PageViewModel PageViewModel { get; set; }//Модель для пагинации списка на странице
        public FilterViewModel FilterViewModel { get; set; }//Модель для фильтрации списка на странице
        public SortState SortViewModel { get; set; }//Модель для сортировки списка на странице
        public IEnumerable<TradeOperation> Operations { get; set; }//Список торговых операций(User)
        public Person Person { get; set; }//Авторизированный пользователь
    }
}
