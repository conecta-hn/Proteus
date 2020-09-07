﻿using System;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class SerialNum : ModelBase<string>
    {
        public string Notes { get; set; }
        public virtual Cliente Owner { get; set; }
        public DateTime? Sold { get; set; }
        public override string ToString()
        {
            return Id;
        }
    }
}