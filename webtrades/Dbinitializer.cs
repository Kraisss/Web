using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webtrades.Models;
using Microsoft.EntityFrameworkCore;


namespace webtrades
{
    public class Dbinitializer
    {
        public static void Initialize(webcontext context)
        {
            if (!context.Items.Any())//Если бд пуста вносим в нее начальные данные
            {
                context.Items.AddRange(
                    new Item
                    {
                        Name = "BeepCoin",
                        ExchangeRate=7400.00
                    },
                    new Item
                    {
                        Name = "Dollar",
                        ExchangeRate=100.00
                    },
                    new Item
                    {
                        Name = "EuroCoin",
                        ExchangeRate=150.00
                        
                    },
                    new Item
                    {
                        Name = "Rubol",
                        ExchangeRate = 10.00

                    },
                    new Item
                    {
                        Name = "OilBarrel",
                        ExchangeRate = 20.00

                    }
                );
                context.SaveChanges();
            }
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role
                    {
                        Name = "Admin"
                       
                    },
                    new Role
                    {
                        Name = "User"
                    }
                    
                );
                context.SaveChanges();
            }
            if (!context.ExchangeRateHistories.Any())
            {
                List<Item> itemslist = context.Items.ToList();
               
                DateTime[] arr = new DateTime[5];
                arr[0] = new DateTime(2015, 5, 17, 18, 30, 25);
                arr[1] = new DateTime(2018, 1, 9, 16, 50, 20);
                arr[2] = new DateTime(2019, 10, 24, 18, 30, 25);
                arr[3] = new DateTime(2020, 1, 2, 13, 24, 25);
                arr[4] = new DateTime(2020, 4, 13, 2, 2, 25);
                foreach (Item i in itemslist)
                {
                    Random random = new Random();
                    double db = i.ExchangeRate;
                    for (int j=0; j<4; j++)
                    {

                        ExchangeRateHistory erh = new ExchangeRateHistory
                        {
                            Item = i,
                            ItemId = i.Id,
                            DateOfChange = arr[j],
                            ExchangeRateChange = Math.Round(random.NextDouble() * (db + (db/5) - (db - (db/5)) + (db - (db/5))),2)
                            
                        };
                        context.ExchangeRateHistories.Add(erh);
                        i.RateHistory.Add(erh);
                        context.SaveChanges();
                            
                    }
                    ExchangeRateHistory erhf = new ExchangeRateHistory
                    {
                        Item = i,
                        ItemId = i.Id,
                        DateOfChange = arr[4],
                        ExchangeRateChange = i.ExchangeRate
                    };
                    context.ExchangeRateHistories.Add(erhf);
                    i.RateHistory.Add(erhf);
                    context.SaveChanges();

                }
                
            }
            //if (!context.People.Any())
            //{
            //    String str = PasswordHash.CreateSalt();
            //    String str1 = PasswordHash.CreateSalt();
            //    string pw1 = "12345ba";
            //    context.People.Add(
            //        new Person
            //        {
            //            Login="Roman1",
            //            PasswordSalt=str,
            //            PasswordHash= PasswordHash.GetHash(str + pw1),
            //            RoleId=2,
            //            Level=0,
            //            PersonalAccount=1.00
            //        }

            //    );
            //    context.SaveChanges();
            //    context.People.Add(
            //        new Person
            //        {
            //            Login = "Elmir1",
            //            PasswordSalt = str1,
            //            PasswordHash = PasswordHash.GetHash(str1+pw1),
            //            RoleId = 1,
            //            Level = 1,
            //            PersonalAccount = 100.32
            //        }

            //    );
            //    context.SaveChanges();
            //}
        }
    }
}
