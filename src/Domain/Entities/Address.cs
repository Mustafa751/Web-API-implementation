using System;
using System.Collections.Generic;

namespace MyPosTask.Domain.Entities
{
    public partial class Address
    {
        public int Id { get; set; }
        public string Address1 { get; set; } = null!;
        public virtual ICollection<PersonAddress> PersonAddresses { get; set; } = new List<PersonAddress>();
    }
}
