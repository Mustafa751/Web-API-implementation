using System;
using System.Collections.Generic;

namespace MyPosTask.Domain.Entities
{
    public partial class PhoneNumber
    {
        public int Id { get; set; }
        public int PersonAddressId { get; set; }
        public string PhoneNumber1 { get; set; } = null!;
        public virtual PersonAddress PersonAddress { get; set; } = null!;
    }
}
