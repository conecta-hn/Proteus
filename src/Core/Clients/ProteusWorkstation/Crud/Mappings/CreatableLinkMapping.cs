/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.Models.Base;
using System.Linq;
using System.Windows.Controls;
using TheXDS.MCART.Types;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Data;
using TheXDS.MCART.Controls;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using System.Threading.Tasks;
using System.Data.Entity;
using SourceChord.FluentWPF;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class CreatableLinkMapping : PropertyMapping, IRefreshable
    {
        private readonly ComboBox _selector = new ComboBox();
        private readonly ToggleButton _btnNew = new ToggleButton { Content = "+" };
        private readonly CrudElement _crud;
        private readonly ILinkPropertyDescription _descr;
        private readonly IQueryable<ModelBase> _source;
        private readonly Service _svc;

        public CreatableLinkMapping(IPropertyDescription property) : base(property, new DockPanel())
        {
            if (!(property is ILinkPropertyDescription i)) return;
            _descr = i;
            _svc = Proteus.InferService(i.Model);
            _source = i.Source;
            _selector.ItemsSource = i.Source.ToList();
            _selector.SelectedValuePath = "Id";
            _selector.DisplayMemberPath = i.DisplayMemberPath;
            _btnNew.Click += BtnNew_Click;

            _crud = new CrudElement(i.Model, null);


            var btnOk = new Button { Content="Aceptar" };
            var btnCancel = new Button { Content = "Cancelar" };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;

            var btns = new StretchyWrapPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Children = { btnOk, btnCancel },
                Style = Application.Current.TryFindResource("Botonera") as Style
            };
            Grid.SetRow(btns, 1);
            var pRoth = new Grid
            {
                Style = Application.Current.TryFindResource("Botonera") as Style,
                Margin = new Thickness(20),
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition{ Height = GridLength.Auto },
                },
                Children = { _crud.Editor, btns }
            };
            var popup = new AcrylicPopup()
            {
                AllowsTransparency = true,
                Child = pRoth
            };
            popup.SetBinding(Popup.IsOpenProperty, new Binding() {
                Path = new PropertyPath(ToggleButton.IsCheckedProperty),
                Source = _btnNew
            });
            DockPanel.SetDock(_btnNew, Dock.Right);
            ((DockPanel)Control).Children.Add(popup);
            ((DockPanel)Control).Children.Add(_btnNew);
            ((DockPanel)Control).Children.Add(_selector);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _crud.ViewModel.Entity = null;
            _crud.ClearAll();
            _btnNew.IsChecked = false;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            _crud.ClearAll();
            _crud.ViewModel.Entity = _descr.Model.New();
            _btnNew.IsChecked = true;
        }

        private async void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            await _svc.AddAsync(_crud.ViewModel.Entity as ModelBase);
            await RefreshAsync();
            _selector.SelectedItem = _crud.ViewModel.Entity;
            BtnCancel_Click(sender, e);
        }

        public override object ControlValue
        {
            get => _selector.SelectedValue;
            set => _selector.SelectedValue = value;
        }

        public override void ClearControlValue() => _selector.SelectedItem = null;

        public void Refresh() => RefreshAsync().GetAwaiter().GetResult();
        public async Task RefreshAsync()
        {
            _selector.ItemsSource = null;
            _selector.ItemsSource = await _source.ToListAsync();
            if (_selector.SelectedItem is ModelBase m && !await _source.ContainsAsync(m)) ClearControlValue();
        }
    }
}