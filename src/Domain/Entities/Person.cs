using System;
using System.Collections.Generic;

namespace MyPosTask.Domain.Entities
{
    public partial class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<PersonAddress> PersonAddresses { get; set; } = new List<PersonAddress>();
    }
}
