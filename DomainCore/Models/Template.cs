using System;

namespace DomainCore.Models
{
    public class Template : BaseModel
    {
        public string Name { get; set; }
        
        public Guid CategoryId { get; set; }
        public Guid InputTypeId { get; set; }
        public Category Category { get; set; }
        public InputType InputType { get; set; }
    }
}
