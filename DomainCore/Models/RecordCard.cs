using System;

namespace DomainCore.Models
{
    public class RecordCard : BaseModel
    {
        public string Value { get; set; }

        public Guid CardId { get; set; }
        public Guid TemplateId { get; set; }
        public Card Card { get; set; }
    }
}
