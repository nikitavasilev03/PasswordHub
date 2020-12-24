using DomainCore.Models;
using System.Collections.Generic;

namespace PasswordHub.ViewModels
{
    public class CategoryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; }
        
        
        
        public Category Category { get; set; }
        public Dictionary<string, InputType> Fields { get; set; }
        public IEnumerable<InputType> InputTypes { get; set; }
    }
}
