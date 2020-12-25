using DomainCore.Context;
using DomainCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordHub.Models;
using PasswordHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        #region Category

        [HttpPost]
        [Route("GetCategoryList")]
        public JsonResult GetCategoryList()
        {
            var data = db.Categories
                .Where(c => c.UserId == UserId)
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
            Category category;
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
                    .ForEachAsync(item =>
                    {
                        //db.RecordCards.RemoveRange(db.RecordCards.Where(r => r.TemplateId == item.Id));
                        db.Templates.Remove(item);
                    });
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
                .ForEachAsync(item =>
                {
                    db.Templates.Remove(item);
                });
            db.Cards.RemoveRange(db.Cards.Where(card => card.CategoryId == category.Id));
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return Json(new JResult(true, "Удаление успешно"));
        }
        
        #endregion

        #region Cards

        [HttpPost]
        [Route("GetCardList")]
        public JsonResult GetCardList(string filter)
        {
            IQueryable<Card> cards;
            if (filter != null)
            {
                Guid guid = new Guid(filter);
                cards = db.Cards.Where(c => c.UserId == UserId && c.CategoryId == guid);
            }
            else
                cards = db.Cards.Where(c => c.UserId == UserId);

           
            var data = cards
                .Where(c => c.UserId == UserId)
                .Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name ?? "",
                    FirstField = db.RecordCards.First(r => r.CardId == c.Id).Value ?? "",
                    CategoryName = db.Categories.First(cat => cat.Id == c.CategoryId).Name ?? "",
                    DateTimeCreate = c.DateTimeCreate.ToString("dd-MM-yyyy HH:mm:ss")
                });
            return Json(data);
        }

        [HttpPost]
        [Route("GetCardJson")]
        public JsonResult GetCardJson(string id)
        {
            var c = db.Cards.First(c => c.UserId == UserId); 
            var data = new
                {
                    Id = c.Id,
                    Name = c.Name ?? "",
                    FirstField = db.RecordCards.First(r => r.CardId == c.Id).Value ?? "",
                    CategoryName = db.Categories.First(cat => cat.Id == c.CategoryId).Name ?? "",
                    DateTimeCreate = c.DateTimeCreate.ToString("dd-MM-yyyy HH:mm:ss")
                };
            return Json(data);
        }

        [HttpGet]
        [Route("CreateCard")]
        public IActionResult CreateCard()
        {
            CardViewModel model = new CardViewModel()
            {
                Name = "",
                Category = null,
                Categories = db.Categories.Where(c => c.UserId == UserId)
            };
            
            return View("Card", model);
        }
        
        [HttpPost]
        [Route("GetFieldForCard")]
        public async Task<JsonResult> GetFieldForCard(string categoryId)
        {
            if (categoryId == null)
                return Json(new JResult(false, "Категория не найдена"));
            Guid guid = new Guid(categoryId);
            Category category = await db.Categories.FirstOrDefaultAsync(c => c.Id == guid);
            if (category == null || category.UserId != UserId)
                return Json(new JResult(false, "Категория не найдена"));
            List<object> dict = new List<object>();
            await db.Templates
                .Where(t => t.CategoryId == guid)
                .ForEachAsync(item =>
                {
                    var it = db.InputTypes.First(i => i.Id == item.InputTypeId);
                    dict.Add(new
                    {
                        Id = item.Id.ToString(),
                        Name = item.Name,
                        Type = it.Type,
                        Size = it.Size
                    });
                });
            
            return Json(new JResult(true, "", dict));
        }

        [HttpGet]
        [Route("EditCard")]
        public async Task<IActionResult> EditCard(string id)
        {
            if (id == null)
                return RedirectToAction("Index");
            Guid guid = new Guid(id);
            Card card = await db.Cards.FirstOrDefaultAsync(c => c.Id == guid && c.UserId == UserId);
            if (card == null)
                return RedirectToAction("Index");
            Category category = await db.Categories.FirstAsync(c => c.Id == card.CategoryId);

            var model = new CardViewModel()
            {
                Id = id,
                Name = card.Name,
                Category = category,
                Categories = db.Categories.Where(c => c.UserId == UserId)
            };
            List<RecordTI> recordTis = new List<RecordTI>();
            await db.RecordCards
                .Where(r => r.CardId == guid)
                .ForEachAsync(item =>
                {
                    var template = db.Templates.First(t => t.Id == item.TemplateId);
                    var it = db.InputTypes.First(i => i.Id == template.InputTypeId);
                    recordTis.Add(new RecordTI()
                    {
                        Name = template.Name,
                        Value = item.Value,
                        TemplateId = template.Id.ToString(),
                        Type = it.Type,
                        Size = it.Size
                    });
                });
            model.Records = recordTis;
            
            return View("Card", model);
        }
            
        [HttpPost]
        [Route("EditCard")]
        public async Task<JsonResult> EditCard(string id, string name, string categoryId, string[] values, string[] templatesId)
        {
            if (categoryId == null)
                return Json(new JResult(false, "Категория не найдена"));
            Guid categoryGuid = new Guid(categoryId);
            Category category = await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryGuid);
            if (category == null || category.UserId != UserId)
                return Json(new JResult(false, "Категория не найдена"));
            Card card = null;
            if (id == null)
            {
                card = new Card()
                {
                    Name = name ?? "No name",
                    CategoryId = categoryGuid,
                    UserId = UserId,
                    DateTimeCreate = DateTime.Now
                };
                await db.Cards.AddAsync(card);
                await db.SaveChangesAsync();
            }
            else
            {
                Guid guid = new Guid(id);
                card = await db.Cards.FirstOrDefaultAsync(c => c.Id == guid && c.UserId == UserId);
                if (card == null)
                    return Json(new JResult(false, "Карта не найдена"));
                card.CategoryId = categoryGuid;
                
                await db.RecordCards
                    .Where(r => r.CardId == guid)
                    .ForEachAsync(item => db.RecordCards.Remove(item));
                await db.SaveChangesAsync();
            }

            RecordCard[] recordCards = new RecordCard[values.Length];
            for (int i = 0; i < recordCards.Length; i++)
            {
                Guid templateGuid = new Guid(templatesId[i]);
                var t1 = await db.Templates.FirstAsync(t => t.Id == templateGuid);
                var template = await db.Templates.FirstAsync(t => t.Id == templateGuid && t.CategoryId == categoryGuid);
                recordCards[i] = new RecordCard()
                {
                    Value = values[i],
                    TemplateId = template.Id,
                    CardId = card.Id
                };
            }
            await db.RecordCards.AddRangeAsync(recordCards);
            await db.SaveChangesAsync();

            return Json(new JResult(true, "Изменения успешно применены"));
        }

        [HttpPost]
        [Route("RemoveCard")]
        public async Task<JsonResult> RemoveCard(string id)
        {
            Guid guid = new Guid(id);
            Card card = await db.Cards.FirstOrDefaultAsync(c => c.Id == guid);
            if (card == null)
                return Json(new JResult(false, "Карточка не обнаружена"));
            if (card.UserId != UserId)
                return Json(new JResult(false, "Упс, что-то пошло не так..."));
            await db.RecordCards
                .Where(r => r.CardId == guid)
                .ForEachAsync(item => db.RecordCards.Remove(item));
            db.Cards.Remove(card);
            await db.SaveChangesAsync();
            return Json(new JResult(true, "Удаление успешно"));
        }

        #endregion

    }
}
