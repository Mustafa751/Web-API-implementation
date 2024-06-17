using System;
using System.Collections.Generic;

namespace MyPosTask.Domain.Entities
{
    public class PersonAddress
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; } = null!;
        public int AddressId { get; set; }
        public virtual Address Address { get; set; } = null!;
        public string Type { get; set; } = null!;
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    }
}
