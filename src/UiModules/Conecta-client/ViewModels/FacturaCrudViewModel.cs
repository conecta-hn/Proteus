/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Facturacion.ViewModels
{
    public abstract class FacturaCrudViewModel : DynamicViewModel<Factura>
    {
        public string FacturaNumber => ConectaService.GetFactNum(Entity);
    }
}