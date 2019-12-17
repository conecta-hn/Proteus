using TheXDS.Proteus.Component;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.ViewModels.Base
{
    public interface IPageViewModel
    {
        bool Closeable { get; }
        SimpleCommand CloseCommand { get; }
        ICloseable Host { get; }
        string Title { get; }

        void Close();
    }
}