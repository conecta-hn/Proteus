using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="CaiRango"/>.
    /// </summary>
    public class CaiRangoDescriptor : CrudDescriptor<CaiRango>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="CaiRango"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Rango de facturación");
            NumericProperty(p => p.NumLocal).Range(0, 999).Format("000").Important("Local").AsListColumn("000");
            NumericProperty(p => p.NumCaja).Range(0, 999).Format("000").Important("Caja").AsListColumn("000").Default((short)1);
            NumericProperty(p => p.NumDocumento).Range(0, 99).Format("00").Important("Documento").AsListColumn("00").Default((byte)1);
            NumericProperty(p => p.RangoInicial).Range(0, 99999999).Format("00000000").Important("Rango inicial").AsListColumn("00000000").Default(1);
            NumericProperty(p => p.RangoFinal).Range(0, 99999999).Format("00000000").Important("Rango Final").AsListColumn("00000000").Default(10000);
        }
    }
}