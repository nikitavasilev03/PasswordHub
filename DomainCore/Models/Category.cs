using System;
using System.Collections.Generic;

namespace DomainCore.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        public IEnumerable<Template> Templates { get; set; }
    }
}
