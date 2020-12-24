using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DomainCore.Context;
using DomainCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordHub.Models;
using PasswordHub.ViewModels;

namespace PasswordHub.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class HubController : BaseController
    {
        public Guid UserId { get => new Guid(User.Identity.Name ?? throw new ArgumentException("Пользователь не найден"));}
        
        public HubController(ApplicationContext context) : base(context)
        {

        }
        
        [Route("Index")]
        public IActionResult Index()
        {
            return View("EmptyBody");
        }

        public IActionResult CreateCategory()
        {
            var model = new CategoryViewModel
            {
                Category = null,
                InputTypes = db.InputTypes
            };
            model.Fields = new Dictionary<string, InputType>()
            {
                {"", model.InputTypes.First()}
            };
            return View("Category", model);
        }

        [HttpPost]
        [Route("GetCategoryList")]
        public JsonResult GetCategoryList()
        {
            var data = db.Categories
                .Where(c => c.UserId == new Guid(User.Identity.Name))
                .Select(c => new { c.Id, c.Name});
            return Json(data);
        }

        [HttpGet]
        [Route("EditCategory")]
        public async Task<IActionResult> EditCategory(string id)
        {
            if (id == null)
                return RedirectToAction("Index");
            Guid guid = new Guid(id);
            Category category = await db.Categories.FirstOrDefaultAsync(c => c.Id == guid);
            if (category == null)
                return RedirectToAction("Index");
            if (category.UserId != UserId)
                return RedirectToAction("Index");

            var model = new CategoryViewModel
            {
                Id = id,
                Name = category.Name,
                Category = category,
                InputTypes = db.InputTypes
            };
            Dictionary<string, InputType> dict = new Dictionary<string, InputType>();
            await db.Templates.Where(t => t.CategoryId == guid).ForEachAsync(item =>
            {
                dict.Add(item.Name, db.InputTypes.First(i => i.Id == item.InputTypeId));
            });
            model.Fields = dict;
            return View("Category", model);
        }

        [HttpPost]
        [Route("EditCategory")]
        public async Task<JsonResult> EditCategory(string id, string name, string[] values, string[] types, int[] sizes)
        {
            Category category = null;
            if (id == null)
            {
                category = new Category()
                {
                    UserId = new Guid(User.Identity.Name ?? string.Empty),
                    Name = name
                };
                await db.Categories.AddAsync(category);
                await db.SaveChangesAsync();
            }
            else
            {
                Guid guid = new Guid(id);
                category = await db.Categories.FirstOrDefaultAsync(c => c.Id == guid);
                if (category == null)
                    return Json(new JResult(false, "Категория не обнаружена"));
                if (category.UserId != UserId)
                    return Json(new JResult(false, "Упс, что-то пошло не так..."));
                await db.Templates
                    .Where(t => t.CategoryId == guid)
                    .ForEachAsync(item => db.Templates.Remove(item));
                await db.SaveChangesAsync();
            }

            Template[] templates = new Template[values.Length];
            for (int i = 0; i < templates.Length; i++)
            {
                var ip = await db.InputTypes.FirstAsync(t => t.Type == types[i] && t.Size == sizes[i]);
                templates[i] = new Template()
                {
                    Name = values[i] ?? "",
                    InputTypeId = ip.Id,
                    CategoryId = category.Id
                };
                await db.Templates.AddAsync(templates[i]);
            }
            await db.SaveChangesAsync();

            return Json(new JResult(true, "Изменения успешно применены"));
        }

        [HttpPost]
        [Route("RemoveCategory")]
        public async Task<JsonResult> RemoveCategory(string id)
        {
            Guid guid = new Guid(id);
            Category category = await db.Categories.FirstOrDefaultAsync(c => c.Id == guid);
            if (category == null)
                return Json(new JResult(false, "Категория не обнаружена"));
            if (category.UserId != UserId)
                return Json(new JResult(false, "Упс, что-то пошло не так..."));
            await db.Templates
                .Where(t => t.CategoryId == guid)
                .ForEachAsync(item => db.Templates.Remove(item));
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return Json(new JResult(true, "Удаление успешно"));
        }
    }
}
