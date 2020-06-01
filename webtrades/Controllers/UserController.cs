using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webtrades.Models;

namespace webtrades.Controllers
{
    public class UserController : Controller
    {
        private webcontext db;
        public UserController(webcontext context)
        {
            db = context;
        }

        [Authorize]//Доступно только авторизированным пользователям
        [HttpGet]
        public async Task<IActionResult> ProfileAsync(SortState.SortStates sortState,string name,int page = 1,string cont="Accounts")// обработчик страницы профиля
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            if(role=="User")//Проверяем роль пользователя, если он юзер то
            {
                if (cont == "Operations")//Проверяем выбор пользователя
                {
                    //Выводим все торговые операции совершенные пользователем в порядке возрастания давности
                    Person person = await db.People.Include(u => u.Accounts).ThenInclude(u => u.Item).Include(u => u.Operations).ThenInclude(u => u.Item).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                    //person.Operations.OrderBy(u => u.Id);
                    ViewBag.cont = cont;//Выводим операции по 10 записей на страницу
                    int pageSize = 10;
                    IQueryable<TradeOperation> UsersList = db.TradeOperations.Where(u => u.PersonId == person.Id);
                    UsersList = UsersList.OrderByDescending(s => s.Id);//передаем данные об операциях

                    var count = await UsersList.CountAsync();
                    var items = await UsersList.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                    AdminProfileModel viewModel = new AdminProfileModel//Передаем данные в модель
                    {
                        PageViewModel = new PageViewModel(count, page, pageSize),
                        Person = person,

                        Operations = items
                    };

                    //ViewBag.login = person.Login;
                    //ViewBag.personalaccount = person.PersonalAccount;

                    return View(viewModel);
                }
                else
                {
                    //Выводим балансы товаров для пользователя
                    Person cperson = await db.People.Include(u => u.Accounts).ThenInclude(u => u.Item).Include(u => u.Operations).ThenInclude(u => u.Item).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                    ViewBag.cont = cont;
                    //ViewBag.login = cperson.Login;
                    //ViewBag.personalaccount = cperson.PersonalAccount;
                    AdminProfileModel viewModel = new AdminProfileModel//загружаем данные по пользователю в модель
                    {
                        Person = cperson                 //SortViewModel = new SortState(sortState),
                        //FilterViewModel = new FilterViewModel(name),
                        
                    };
                    return View(viewModel);//Возвращаем представление
                }
            }
            else//Если роль Admin
            {
                //Выводим список User-ов по 10 штук на страницу
                int pageSize = 10;
                IQueryable<Person> UsersList = db.People.Where(u => u.role.Name == "User");
                if (!String.IsNullOrEmpty(name))
                {
                    UsersList = UsersList.Where(p => p.Login.Contains(name));
                }
                switch (sortState)//Сортировка
                {
                    case SortState.SortStates.LoginDesc:
                        UsersList = UsersList.OrderByDescending(s => s.Login);
                        break;
                    case SortState.SortStates.LevelAsc:
                        UsersList = UsersList.OrderBy(s => s.Level);
                        break;
                    case SortState.SortStates.LevelDesc:
                        UsersList = UsersList.OrderByDescending(s => s.Level);
                        break;
                    default:
                        UsersList = UsersList.OrderBy(s => s.Login);
                        break;
                }
                var count = await UsersList.CountAsync();
                var items = await UsersList.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                AdminProfileModel viewModel = new AdminProfileModel//Заносим данные в модель
                {
                    PageViewModel = new PageViewModel(count, page, pageSize),
                    SortViewModel = new SortState(sortState),
                    FilterViewModel = new FilterViewModel(name),
                    Users = items
                };
                //List<Person> UsersList = db.People.Where(u => u.role.Name == "User").ToList();
                //ViewData["LoginSort"] = sortState == SortState.SortStates.LoginAsc ? SortState.SortStates.LoginDesc : SortState.SortStates.LoginAsc;
                //ViewData["LevelSort"] = sortState == SortState.SortStates.LevelAsc ? SortState.SortStates.LevelDesc : SortState.SortStates.LevelAsc;
                //ViewData["CompSort"] = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
                Person person = await db.People.Include(u => u.role).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                ViewBag.login = person.Login;//Заносим данные о пользователе в бэги
                ViewBag.personalaccount = person.PersonalAccount;
                ViewBag.role = person.role.Name;
                

                //UsersList = sortState switch
                //{
                //    SortState.SortStates.LoginDesc => UsersList.OrderByDescending(s => s.Login),
                //    SortState.SortStates.LevelAsc => UsersList.OrderBy(s => s.Level),
                //    SortState.SortStates.LevelDesc => UsersList.OrderByDescending(s => s.Level),
                //    _ => UsersList.OrderBy(s => s.Login),
                //};

                return View("ProfileAdmin",viewModel);//выводим представление
            }
            
        }

        [Authorize(Roles = "User")]//Доступно только авторизированным User-ам
        [HttpGet]
        public async Task<IActionResult> AddWithdrawFundsAsync()//Вывод страницы пополнения/вывода средств
        {
            //UserProfileModel upm= new UserProfileModel();
            //Person person = await db.People.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            //upm.person = person;
            //upm.ItemPersonAccountList= db.ItemPersonAccounts.Where(u => u.PersonId == person.Id).ToList();
            Person person = await db.People.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            ViewBag.login = person.Login;//Вносим в бэги информацию о пользователи
            ViewBag.personalaccount = person.PersonalAccount;
            return View();
        }
        [Authorize(Roles = "User")]//Доступно только авторизированным User-ам
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWithdrawFundsAsync(AddWithdrawModel model)// обработчик пополнения/вывода средств
        {
            //UserProfileModel upm= new UserProfileModel();
            // Person person = await db.People.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            //upm.person = person;
            //upm.ItemPersonAccountList= db.ItemPersonAccounts.Where(u => u.PersonId == person.Id).ToList();
            Person person = await db.People.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            if (ModelState.IsValid)//Если модель верна
            {
                double d = Convert.ToDouble(model.Amount);
                if (d > 0.00)//Если введенная сумма больше 0
                {
                    if (model.Action == "Add")//Если User хочет пополнить средства
                    {
                        person.PersonalAccount += d;//добавляем ему средства на счет
                        await db.SaveChangesAsync();//Сохраняем изменения в бд и возвращаем в профиль
                        return RedirectToAction("Profile", "User");
                    }
                    if (model.Action == "Withdraw")//Если User хочет вывести средства
                    {
                        if (d > person.PersonalAccount)//Если введенная сумма больше того что есть у пользователя
                        {
                            ModelState.AddModelError("", "Не достаточно средств для вывода");
                            //Person person = await db.People.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
                            ViewBag.login = person.Login;
                            ViewBag.personalaccount = person.PersonalAccount;
                            return View(model);//выводим ошибку и возвращаем представление
                        }
                        else
                        {
                            person.PersonalAccount -= d;//Снимаем средства с счета пользователя и возвращаем в профиль
                            await db.SaveChangesAsync();//сохраняем изменения в бд
                            return RedirectToAction("Profile", "User");
                        }
                    }
                }
                //Если введенная сумма <=0
                ModelState.AddModelError("", "Сумма не может быть отрицательной или равной нулю");
            }
            ViewBag.login = person.Login;
            ViewBag.personalaccount = person.PersonalAccount;
            return View(model);//Если модель неверна возвращаем представление
        }
        [Authorize(Roles = "Admin")]//Доступно только авторизированному Admin-у
        public async Task<IActionResult> LevelChangeAsync(SortState.SortStates sortState, string name, string pers, int page = 1)//обработчик изменения уровня доступа User-a
        {
            Person cperson = await db.People.FirstOrDefaultAsync(u => u.Login == pers);
            if (cperson.Level == 1)//Находим выбранного пользователя и проверяем его уровень доступа, меняем его на противоположный
                cperson.Level = 0;
            else
                cperson.Level = 1;
            db.People.Update(cperson);
            await db.SaveChangesAsync();//Сохраняем данные  в бд
            Person person = await db.People.Include(u => u.role).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);
            ViewBag.login = person.Login;
            ViewBag.personalaccount = person.PersonalAccount;
            ViewBag.role = person.role.Name;//Заносим в бэги данные по Admin-у
            return RedirectToAction("Profile","User",new { sortState, name, page });//Возвращаем представление с выбранными настройками
        }
        //[HttpPost]
        //public IActionResult AddWithdrawFunds(AddWithdrawModel model)
        //{
        //    ViewBag.personalaccount = model.Amount;
        //    ViewBag.login = "DiCK";
        //    return View();
        //}
    }
}