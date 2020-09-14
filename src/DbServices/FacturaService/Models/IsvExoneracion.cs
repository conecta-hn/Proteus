using System;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class IsvExoneracion : TimestampModel<string>
    {
        public DateTime Void { get; set; }

        public virtual IsvExoneracionType Kind { get; set; } = null!;
    }
}