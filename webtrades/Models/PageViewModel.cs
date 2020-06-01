using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class PageViewModel
    {
        public int PageNumber { get; private set; }//Модель для пагинации страниц(разбиение на страницы списка)
        public int TotalPages { get; private set; }

        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}
