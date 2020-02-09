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

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Interaction logic for FacturadorPage.xaml
    /// </summary>
    public partial class FacturadorPage : ProteusPage
    {
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
        }

        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TxtNewItemCode.Focus();
        }
    }
}
