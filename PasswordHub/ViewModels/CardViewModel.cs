using DomainCore.Models;
using System.Collections.Generic;
using PasswordHub.Models;

namespace PasswordHub.ViewModels
{
    public class CardViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }

        public Category Category { get; set; }
        
        public IEnumerable<RecordTI> Records { get; set; }
        
        public IEnumerable<Category> Categories { get; set; }
    }
}
