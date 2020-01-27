/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;
using System.Collections.Generic;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Component
{
    public interface ISettingsRepository : IAsyncDbSeeder, IExposeGuid, INameable
    {
        Setting this[string customSetting] { get; set; }

        IEnumerable<Setting> Settings { get; }
    }
}