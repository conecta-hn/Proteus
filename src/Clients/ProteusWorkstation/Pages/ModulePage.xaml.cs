using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.Widgets;
using System.Linq;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para ModulePage.xaml
    /// </summary>
    public partial class ModulePage : IPageVisualHost
    {        
        public new ModulePageViewModel ViewModel => base.ViewModel as ModulePageViewModel;
        public UiModule Module { get; }
        public ModulePage(UiModule module)
        {
            InitializeComponent();
            base.ViewModel = new ModulePageViewModel(this)
            {
                Module = module
            };
            Module = module;
            module.Host = ViewModel;
        }

        public void Activate(IPage page)
        {
            TabHost.SelectedItem = page;
            Activate();
        }

        public override void Close()
        {
            while (ViewModel.Pages.Any()) ViewModel.Pages.First().Close();
            base.Close();
        }
    }
}