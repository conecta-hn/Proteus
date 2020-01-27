/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ConfigRepository : ModelBase<Guid>
    {
        public virtual List<Setting> Settings { get; set; } = new List<Setting>();
    }
}