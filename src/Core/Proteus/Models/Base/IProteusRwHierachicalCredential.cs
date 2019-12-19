/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusRwHierachicalCredential : IProteusRwCredential
    {
        UserGroup Parent { get; set; }
        ICollection<UserRole> Roles { get; }
    }
}