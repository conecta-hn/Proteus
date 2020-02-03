/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;
using System.Collections.Generic;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Define una serie de miembros a implementar por un servicio que
    /// ofrezca acceso a valores de configuración.
    /// </summary>
    public interface IConfigurableService : INameable
    {
        IEnumerable<Setting> Settings { get; }
    }
}