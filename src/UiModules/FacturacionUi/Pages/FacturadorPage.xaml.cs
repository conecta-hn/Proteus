/*
Copyright © 2017-2019 César Andrés Morgan
Desarrollado bajo el auspicio de Grupo Sinergia S. de R. L. como un ERP para
Laboratorios Médicos, S. de R. L.. Propiedad intelectual de César Andrés Morgan
Licenciado para uso interno solamente.
===============================================================================
Este archivo forma parte de SLM. Su uso debe limitarse a la implementación de
herramientas internas para Laboratorios Médicos S. de R. L. El contenido de
este archivo no debe ser distribuido en ningún producto comercial ni debe ser
reutilizado con otros fines no relacionados. El autor se absuelve de toda
responsabilidad y daños causados por el uso indebido de este archivo o de
cualquier parte de su contenido.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.FacturacionUi.Component;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.Crud.Base;
using System.Windows.Data;
using System.Linq;
using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;
using System;
using System.Reflection;
using TheXDS.Proteus.Crud;
using System.Windows;
using TheXDS.MCART.Types;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Interaction logic for FacturadorPage.xaml
    /// </summary>
    public partial class FacturadorPage : ProteusPage
    {
        private struct ClientePd : IObjectPropertyDescription
        {
            private readonly FacturadorViewModel _vm;

            public bool Selectable => true;

            public BindingBase DisplayMemberBinding => new Binding("Name");

            public string DisplayMemberPath => "Name";

            public IQueryable<ModelBase> Source => _vm.Clientes.AsQueryable();

            public bool UseVmSource => false;

            public bool Creatable => true;

            public IEnumerable<Type> ChildModels
            {
                get
                {
                    yield return typeof(Cliente);
                }
            }

            public PropertyLocation PropertySource => PropertyLocation.ViewModel;

            public PropertyInfo Property => typeof(FacturadorViewModel).GetProperty(nameof(FacturadorViewModel.NewCliente))!;

            public Type PropertyType => typeof(Cliente);

            public object Default => null;

            public bool Hidden => false;

            public bool ReadOnly => false;

            public bool ShowInDetails => false;

            public bool ShowWatermark => true;

            public string ReadOnlyFormat => null;

            public string Icon => "🧑";

            public string Label => "Cliente";

            public NullMode Nullability => NullMode.Required;

            public string RadioGroup => null!;

            public int? Order => null;

            public Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>> Validator => (e, p) => Array.Empty<ValidationError>();

            public string Tooltip => "Seleccione un cliente";

            public IDictionary<DependencyProperty, BindingBase> CustomBindings => new Dictionary<DependencyProperty, BindingBase>();

            public bool IsListColumn => true;

            public bool UseDefault => false;

            public ObservableListWrap<ModelBase>? VmSource(object parentVm, CrudViewModelBase? elementVm)
            {
                throw new NotImplementedException();
            }

            public ClientePd(FacturadorViewModel vm)
            {
                _vm = vm;
            }
        }

        public FacturadorPage() : this((Factura?)null) { }

        public FacturadorPage(IFacturaUIInteractor? interactor) : this(null, interactor) { }

        public FacturadorPage(Factura? factura) : this(factura, null) { }

        public FacturadorPage(Factura? factura, IFacturaUIInteractor? interactor)
        {
            InitializeComponent();

            if (FacturaService.GetEstation?.SecondScreen.HasValue ?? false)
            {
                var f2nd = new Fact2ndScreen();
                var cg = new CloseableGroup(new ICloseable[] { this, f2nd });
                ViewModel = new FacturadorViewModel(cg, interactor, factura);
                f2nd.DataContext = ViewModel;
                f2nd.Show();
            }
            else
            {
                ViewModel = new FacturadorViewModel(this, interactor, factura);
            }
            ClienteSelector.DataContext = new ObjectEditorViewModel((FacturadorViewModel)ViewModel, new ClientePd((FacturadorViewModel)ViewModel), typeof(Cliente));
            if (!(factura is null))
            {
                ((ObjectEditorViewModel)ClienteSelector.DataContext).Selection = factura.Cliente;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            TxtNewItemCode.Focus();
        }
    }
}
