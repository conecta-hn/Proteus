/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;

namespace TheXDS.Proteus.Pages.Base
{
    public abstract class ProteusPage : TabItem, IPage
    {
        protected PageViewModel ViewModel
        {
            get => DataContext as PageViewModel;
            set
            {
                DataContext = value;
            }
        }
        public IPageHost PageHost { get; set; }

        public void Activate()
        {
            if (PageHost is null)
            {
                App.RootHost.OpenPage(this);
            }
            PageHost.SwitchTo(this);
        }

        public virtual void Close()
        {
            PageHost?.ClosePage(this);
            PageHost = null;
        }
        private static Thickness _thk = new Thickness(10,0,0,0);
        public ProteusPage()
        {
            ViewModel = new NullPageViewModel(this);
            Style = Application.Current.TryFindResource("TabItemBase") as Style;
            var cb = new Button
            {
                VerticalAlignment = VerticalAlignment.Center,
                Content = "x",
                Margin=_thk,
                Style = Application.Current.TryFindResource("EmbossBtn") as Style
            };
            cb.SetBinding(ButtonBase.CommandProperty, new Binding(nameof(PageViewModel.CloseCommand)));
            var title = new TextBlock();
            title.SetBinding(TextBlock.TextProperty, new Binding(nameof(PageViewModel.Title)));
            DockPanel.SetDock(cb, Dock.Right);
            Header = new DockPanel
            {
                Children = { title, cb }
            };
        }
    }
}
