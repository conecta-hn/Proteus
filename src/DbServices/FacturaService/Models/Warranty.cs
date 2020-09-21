using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class Warranty : TimestampModel<long>
    {
        public virtual Cliente Cliente { get; set; }
        public DateTime? Void { get; set; }
        public virtual List<SerialNum> Items { get; set; } = new List<SerialNum>();
    }
}