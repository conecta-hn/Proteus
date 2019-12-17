using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Pages.Base
{
    public interface ILoginPage : IPage
    {
        LoginViewModel ViewModel { get; }
    }
}