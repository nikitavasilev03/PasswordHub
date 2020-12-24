using DomainCore.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordHub.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationContext db;

        public BaseController(ApplicationContext context)
        {
            db = context;
        }
    }
}
