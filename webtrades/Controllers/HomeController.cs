using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webtrades.Models;

namespace webtrades.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private webcontext db;

        public HomeController(ILogger<HomeController> logger,webcontext context)
        {
            _logger = logger;
            db = context;
        }
        [Authorize] //Доступно только авторизированным пользователям
        public async Task<IActionResult> IndexAsync(string item, string msg) //главная страница
        {
            HomeModel model = new HomeModel();// создаем модель представления и вносим в нее данные
            model.items = await db.Items.ToListAsync();
            model.person = await db.People.Include(u => u.role).Include(u=>u.Accounts).FirstOrDefaultAsync(u=> u.Login==User.Identity.Name);
            if(item!=null)// если был выбран товар
            {
                model.itemchoosed=await db.Items.FirstOrDefaultAsync(u=>u.Name==item);
                //model.personAccount = await db.ItemPersonAccounts.FirstOrDefaultAsync(u => u.ItemId == model.itemchoosed.Id);
                model.personAccount =  model.person.Accounts.FirstOrDefault(u=>u.ItemId==model.itemchoosed.Id);
                IQueryable<ExchangeRateHistory> list = db.ExchangeRateHistories.Where(u => u.ItemId == model.itemchoosed.Id);
                list = list.OrderBy(u => u.Id);//Продолжаем заполнять модель, вносим в нее товар, историю курса, баланс пользователя товара
                var histories = list.ToList();
                List<string> dates = new List<string>();
                List<double> rates = new List<double>();
                foreach(ExchangeRateHistory ert in histories)
                {
                    dates.Add(ert.DateOfChange.ToString());
                    rates.Add(Math.Round(ert.ExchangeRateChange,2));
                }
                ViewBag.dates = dates;//Бэги используем для внесения данных в график в представлении
                ViewBag.rates = rates;
                model.History = histories;

            }
            if(msg!=null)//Если есть сообщение об ошибке передаем ее в представление
            {
                ViewBag.msg = msg;
            }
            

            return View(model);
        }
        [Authorize(Roles ="User")]//Доступно только авторизированному User-у
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyAsync(HomeModel model,string item,string login)//Покупка товара
        {
            string msg;
            if (ModelState.IsValid)//Если модель данных верна
            {
                Person person = await db.People.FirstOrDefaultAsync(u => u.Login == login);
                if (person.Level != 0)//Находим пользователя по логину и смотрим его уровень доступа к операциям
                {
                    Item iteme = await db.Items.FirstOrDefaultAsync(u => u.Name == item);//Берем выбранный товар и находим его баланс этого пользователя
                    ItemPersonAccount ipa = await db.ItemPersonAccounts.FirstOrDefaultAsync(u => u.PersonId == person.Id && u.ItemId == iteme.Id);
                    double d = Convert.ToDouble(model.Amount);
                    if (d > 0)//Проверяем количество товара
                    {
                        if (d * iteme.ExchangeRate <= person.PersonalAccount)//Проверяем хватает ли средств у пользователя для покупки введенного ко-ва товара
                        {
                            person.PersonalAccount -= d * iteme.ExchangeRate;//Если да то снимаем средства у пользователя , и начисляем товар на баланс
                            ipa.ItemQuantity += d;
                            TradeOperation to = new TradeOperation();//Создаем запись о выполненной операции 
                            to.ItemId = iteme.Id;
                            to.PersonId = person.Id;
                            to.OperationType = "Buy";
                            to.Profit = d * iteme.ExchangeRate;
                            to.DateOfOperation = DateTime.Now;
                            await db.TradeOperations.AddAsync(to);
                            person.Operations.Add(to);
                            await db.SaveChangesAsync();//Сохраняем изменения в бд
                            return RedirectToAction("Index", "Home", new { item });//Возвращаемся на главную страницу с выбранным товаром
                        }
                        else//Если не хватает средств
                        {
                            //ModelState.AddModelError("", "У вас не хватает средств");
                            msg = "У вас не хватает средств";
                            return RedirectToAction("Index", "Home", new { item, msg });
                        }
                    }
                    else//Если количество товара <=0
                    {
                        msg = "Введите количество товара";
                        return RedirectToAction("Index", "Home", new { item, msg });
                    }
                }
                else//Если закрыт доступ к торговым операциям
                {

                    //ModelState.AddModelError("", "Действие заблокировано для вас");
                    msg = "Действие заблокировано для вас";
                    return RedirectToAction("Index", "Home", new { item, msg });
                }

            }

            //ModelState.AddModelError("", "Неверные данные");
            msg = "Неверные данные";//Если модель данных неверна
            return RedirectToAction("Index", "Home", new { item,msg });

        }

        [Authorize(Roles = "User")]//Доступно только авторизированному пользователю
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellAsync(HomeModel model, string item, string login)
        {
            string msg;
            if(ModelState.IsValid)//Проверяем правильность введенных данных
            {
                Person person = await db.People.FirstOrDefaultAsync(u => u.Login == login);
                if (person.Level != 0)//Находим пользователя и проверяем его уровень доступа к операциям
                {
                    Item iteme = await db.Items.FirstOrDefaultAsync(u => u.Name == item);//Находим выбранный товар и его баланс этого пользователя
                    ItemPersonAccount ipa = await db.ItemPersonAccounts.FirstOrDefaultAsync(u => u.PersonId == person.Id && u.ItemId == iteme.Id);
                    double d = Convert.ToDouble(model.Amount);
                    if (d > 0)//Если введенное количество товара больше нуля
                    {
                        if (ipa.ItemQuantity >= d)//Проверяем хватает ли у пользователя товара для продажи такого количества
                        {
                            person.PersonalAccount += d * iteme.ExchangeRate;//Если да начисляем средства пользователю, снимаем кол-во товара с баланса пользователя
                            ipa.ItemQuantity -= d;
                            TradeOperation to = new TradeOperation();//Создаем запись об операции продажи
                            to.ItemId = iteme.Id;
                            to.PersonId = person.Id;
                            to.OperationType = "Sell";
                            to.Profit = d * iteme.ExchangeRate;
                            to.DateOfOperation = DateTime.Now;
                            await db.TradeOperations.AddAsync(to);
                            person.Operations.Add(to);
                            await db.SaveChangesAsync();//Сохраняем изменения в бд
                            return RedirectToAction("Index", "Home", new { item });//Перенаправляем на главную стр. с выбранным товаром
                        }
                        else//Если у User-a не хватает товара для продажи
                        {
                            //ModelState.AddModelError("", "Не хватает товара");
                            msg = "Не хватает товара";
                            return RedirectToAction("Index", "Home", new { item, msg });
                        }
                    }
                    else//Если введено количество товара <=0
                    {
                        msg = "Введите количество товара";
                        return RedirectToAction("Index", "Home", new { item, msg });
                    }
                }
                else//Если доступ к операциям заблокирован
                {
                   // ModelState.AddModelError("", "Действие заблокировано для вас");
                    msg = "Действие заблокировано для вас";
                    return RedirectToAction("Index", "Home", new { item,msg });
                }
            }
            //ModelState.AddModelError("", "Неверные данные");
            msg = "Неверные данные";//Если модель данных неверна
            return RedirectToAction("Index", "Home", new { item,msg });
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()//Метод для вывода ошибок
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
