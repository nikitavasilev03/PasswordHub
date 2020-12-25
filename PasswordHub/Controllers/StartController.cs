using System.Threading.Tasks;
using DomainCore.Context;
using DomainCore.Helpers;
using DomainCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordHub.ViewModels;

namespace PasswordHub.Controllers
{
    [Route("")]
    [Route("[controller]")]
    public class StartController : AuthController
    {
        public StartController(ApplicationContext context) : base(context)
        {

        }
        
        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Hub");
            return View("SignIn");
        }
        
        [HttpGet]
        [Route("SingIn")]
        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Hub");
            return View("SignIn");
        }

        [HttpPost]
        [Route("SingIn")]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                    return View(model);
                }
                if (user.Password != Password.Hash(model.Password))
                {
                    ModelState.AddModelError("", "Не верный пароль");
                    return View(model);
                }

                await Authenticate(user.Id);
                return RedirectToAction("Index", "Hub");

            }
            return View(model);
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Hub");
            return View("Register");
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfPassword)
                {
                    ModelState.AddModelError("", "Пароли не совпадают");
                    return View(model);
                }
                var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    ModelState.AddModelError("", "Пользователь с таким адресом уже существует");
                    return View(model);
                }

                var newUser = new User()
                {
                    Email = model.Email,
                    Password = Password.Hash(model.Password),
                    FirstName = "",
                    SecondName = "",
                    LastName = ""
                };

                await db.Users.AddAsync(newUser);
                await db.SaveChangesAsync();
                await Authenticate(newUser.Id);
                
                //Init category
                Category[] categories = new Category[]
                {
                    new Category() { UserId = newUser.Id, Name = "Аккаунты" },
                    new Category() { UserId = newUser.Id, Name = "Заметки" },
                    new Category() { UserId = newUser.Id, Name = "Банковские карты" },
                    new Category() { UserId = newUser.Id, Name = "Паспорта" }
                };
                await db.Categories.AddRangeAsync(categories);
                await db.SaveChangesAsync();

                var category = categories[0];
                //Init Template fields
                Template[] templates0 = new Template[]
                {
                    new Template() { Name = "Имя пользователя", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Пароль", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                };
                category = categories[1];
                Template[] templates1 = new Template[]
                {
                    new Template() { Name = "Содержимое", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 2)},
                };
                category = categories[2];
                Template[] templates2 = new Template[]
                {
                    new Template() { Name = "Название карты", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Номер карты", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Владелец", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Действует до", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "CVC", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                };
                category = categories[3];
                Template[] templates3 = new Template[]
                {
                    new Template() { Name = "Фамилия Имя Отчество", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Дата рождения", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Серия", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Номер", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Кем выдан", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 1)},
                    new Template() { Name = "Когда выдан", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Код подразделения", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                    new Template() { Name = "Действует до", Category = category, InputType = await db.InputTypes.FirstAsync(i => i.Type == "Text" && i.Size == 0)},
                };

                await db.Templates.AddRangeAsync(templates0);
                await db.Templates.AddRangeAsync(templates1);
                await db.Templates.AddRangeAsync(templates2);
                await db.Templates.AddRangeAsync(templates3);
                await db.SaveChangesAsync();

                return RedirectToAction("Index", "Hub");
            }

            return View(model);
        }
    }
}
