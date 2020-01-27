/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.ViewModels.Base
{
    internal class NullPageViewModel : PageViewModel
    {
        public NullPageViewModel(ICloseable host) : base(host)
        {
            Title = "⚠ Página sin inicializar";
        }
    }
}