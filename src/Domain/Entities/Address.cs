using System;
using System.Collections.Generic;
using MyPosTask.Domain.Enums;

namespace MyPosTask.Domain.Entities
{
    public partial class Address
    {
        public int Id { get; set; }
        public int PeopleId { get; set; }
        public string Type { get; set; } = null!; // Change this to string
        public string Address1 { get; set; } = null!;
        public virtual Person People { get; set; } = null!;
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    }
}
