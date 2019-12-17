using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.ViewModels.Base
{
    internal class NullPageViewModel : PageViewModel
    {
        public NullPageViewModel(ICloseable host) : base(host)
        {
            Title = "/!\\ Página sin inicializar!!";
        }
    }
}