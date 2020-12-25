using System;
using System.Collections.Generic;

namespace DomainCore.Models
{
    public class Card : BaseModel
    {
        public string Name { get; set; }
        public DateTime DateTimeCreate { get; set; }


        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CategoryId { get; set; }
        public IEnumerable<RecordCard> RecordCards { get; set; }
    }
}
