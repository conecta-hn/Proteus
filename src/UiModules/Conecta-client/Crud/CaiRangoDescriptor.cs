/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class CaiRangoDescriptor : CrudDescriptor<CaiRango>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Rango de facturación");
            NumericProperty(p => p.NumLocal).Range(0, 999).Format("000").Important("Local");
            NumericProperty(p => p.NumCaja).Range(0, 999).Format("000").Important("Caja");
            NumericProperty(p => p.NumDocumento).Range(0, 99).Format("00").Important("Documento");
            NumericProperty(p => p.RangoInicial).Range(0, 99999999).Format("00000000").Important("Rango inicial");
            NumericProperty(p => p.RangoFinal).Range(0, 99999999).Format("00000000").Important("Rango Final");
        }
    }
}