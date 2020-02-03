/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.ViewModels
{
    public class AvisoCrudViewModel : CrudViewModel<UserService>
    {
        public AvisoCrudViewModel(ICloseable host) : base(host, typeof(Aviso))
        {
        }
    }
}