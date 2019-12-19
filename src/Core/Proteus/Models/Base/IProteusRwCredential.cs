/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using System.Collections.Generic;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusRwCredential : IModelBase<string>, INameable
    {
        new string Name { get; set; }
        SecurityBehavior? ModuleBehavior { get; set; }
        SecurityBehavior? ButtonBehavior { get; set; }
        ICollection<SecurityDescriptor> Descriptors { get; }
        SecurityFlags DefaultGranted { get; set; }
        SecurityFlags DefaultRevoked { get; set; }
    }
}