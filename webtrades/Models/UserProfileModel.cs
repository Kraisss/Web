using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class UserProfileModel
    {
       public Person person { get; set; }//авторизированный User
       public  List<ItemPersonAccount> ItemPersonAccountList { get; set; }//баланс товаров этого User-a
    }
}
