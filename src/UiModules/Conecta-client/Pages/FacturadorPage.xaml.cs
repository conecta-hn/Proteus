/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Component;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Facturacion.ViewModels;

namespace TheXDS.Proteus.Facturacion.Pages
{
    /// <summary>
    /// Interaction logic for FacturadorPage.xaml
    /// </summary>
    public partial class FacturadorPage
    {
        public FacturadorPage() : this(null)
        {
        }

        public FacturadorPage(IFacturaUIInteractor interactor) : this(null, interactor)
        {
        }

        public FacturadorPage(Factura factura, IFacturaUIInteractor interactor)
        {
            InitializeComponent();
            ViewModel = new FacturadorViewModel(this, interactor, factura);
        }

        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TxtNewItemCode.Focus();
        }
    }
}
