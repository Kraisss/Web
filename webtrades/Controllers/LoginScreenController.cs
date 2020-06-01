using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webtrades.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webtrades.Controllers
{
    public class LoginScreenController : Controller
    {
        // GET: /<controller>/
        //[Authorize]
        //public IActionResult IndexTest()
        //{
        //    ViewBag.Person = User.Identity.Name;
        //    List<Item> list= db.Items.ToList();
        //    return View(list);
        //}
        private webcontext db;
        public LoginScreenController(webcontext context)
        {
            db = context;
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckLoginAsync(string login)//Проверка существования пользователя с таким логином
        {
            Person person = await db.People.FirstOrDefaultAsync(u => u.Login == login);
            if(person!=null)
                return Json(false);
            return Json(true);
        }
        [HttpGet]
        public IActionResult Login()//Возвращение представления авторизации
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)//обработчик авторизации
        {
            if (ModelState.IsValid)//Если модель верна
            {
               
                Person user = await db.People.Include(u =>u.role).FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user != null)//Если был найден пользователь с введенным логином
                {
                    string str1 = PasswordHash.GetHash(model.Password + user.PasswordSalt);
                    if (str1 == user.PasswordHash)//хэшируем введенный пароль с солью и проверяем его с хэшэм в бд
                    {
                        string userrole = user.role.Name;
                        await Authenticate(model.Login,userrole); // аутентификация

                        return RedirectToAction("Index", "Home");//Перенаправление на главную страницу
                    }
                    else//Если не совпал хэшированный пароль с солью с хэшем из бд
                    {
                        ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");//Если не найден пользователь с таким логином
            }
            return View(model);//Если модель неверна возвращаем представление
        }
        [HttpGet]
        public IActionResult Register()//Представление регистрации
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)//Обработчик регистрации
        {
            if (ModelState.IsValid)//Если модель верна
            {
                Person user = await db.People.FirstOrDefaultAsync(u => u.Login == model.Login);
                if (user == null)//Проверяем наличие в бд пользователя с введенным логином
                {
                    string str1 = PasswordHash.CreateSalt();//Если такого пользователя нет, создаем соль и хэшируем введенный пароль с ней
                    Role role = await db.Roles.FirstOrDefaultAsync(u => u.Name == "User");// присваиваем роль User
                    List<Item> ItemsList = await db.Items.ToListAsync();
                    // добавляем пользователя в бд
                    db.People.Add(new Person { Login = model.Login, PasswordSalt=str1,PasswordHash = PasswordHash.GetHash( model.Password+str1),RoleId=role.Id,Level=1,PersonalAccount=0.00 });
                    await db.SaveChangesAsync();

                    Person person = await db.People.Include(u=>u.role).FirstOrDefaultAsync(u => u.Login == model.Login);
                    //Item item = await db.Items.FirstOrDefaultAsync(u => u.Id == 1);
                    foreach (Item v in ItemsList)//Для каждого существующего товара создаем баланс товара для этого пользователя
                    {
                        ItemPersonAccount temp = new ItemPersonAccount();
                        temp.PersonId = person.Id;
                        temp.ItemQuantity = 0.00;
                        temp.ItemId = v.Id;
                        db.ItemPersonAccounts.Add(temp);
                        person.Accounts.Add(temp);
                    }
                    //db.ItemPersonAccounts.Add(new ItemPersonAccount { PersonId = person.Id, ItemQuantity = 0.00, ItemId = item.Id });
                    await db.SaveChangesAsync();//сохраняем балансы товаров в бд
                    string personrole = person.role.Name;
                    await Authenticate(model.Login,personrole); // аутентификация

                    return RedirectToAction("Index", "Home");//Перенаправление на главную страницу
                }
                else//Если найден пользователь с таким логином
                    ModelState.AddModelError("", "Пользователь с таким логином уже зарегистрирован");
            }
            return View(model);
        }

        private async Task Authenticate(string userName,string userRole)//Аутентификация 
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()//Удаляем аутентификационный куки и возвращаем на страницу авторизации
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "LoginScreen");
        }
    }
}
