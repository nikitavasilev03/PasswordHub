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
        
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View("SignIn");
        }
        
        [HttpGet]
        [Route("SingIn")]
        public IActionResult SignIn()
        {
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

                await Authenticate(user.Email);

            }
            return View(model);
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
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
                await Authenticate(newUser.Email);
            }

            return View(model);
        }
    }
}
