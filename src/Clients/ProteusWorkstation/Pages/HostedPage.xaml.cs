using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para HostedPage.xaml
    /// </summary>
    public partial class HostedPage
    {
        private class HostedPageViewModel : PageViewModel
        {
            internal HostedPageViewModel(ICloseable host) : base(host)
            {
            }
        }

        private HostedPageViewModel Vm => (HostedPageViewModel)DataContext;

        protected HostedPage()
        {
            InitializeComponent();
            DataContext = new HostedPageViewModel(this);
        }

        public static HostedPage New<T>() where T : FrameworkElement, new() => New<T>(typeof(T).NameOf());
        public static HostedPage New<T>(string title) where T : FrameworkElement, new()
        {
            return new HostedPage().Setup<T>(title);
        }
        public static HostedPage From(Page pg)
        {
            return From(FromPage(pg));
        }
        public static HostedPage From(UIElement fe)
        {
            return new HostedPage() { Content = fe };
        }


        private protected HostedPage Setup<T>(string title) where T : FrameworkElement, new()
        {
            Vm.Title = title;
            Content = typeof(T).New() switch
            {
                Page pg => FromPage(pg),
                Control c => c,
                _ => AppInternal.BuildWarning($"No se ha implementado un host para el {nameof(FrameworkElement)} '{typeof(T).CSharpName()}'")
            };
            return this;
        }

        private static UIElement FromPage(Page pg)
        {
            var f = new Frame 
            {
                NavigationUIVisibility = NavigationUIVisibility.Hidden
            };
            f.Navigate(pg);
            return f;
        }

    }

    public class HostedPage<T> : HostedPage where T: FrameworkElement, new()
    {
        public HostedPage()
        {
            Setup<T>(typeof(T).NameOf());
        }
    }
}
