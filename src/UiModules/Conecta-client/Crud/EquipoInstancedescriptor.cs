/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class EquipoInstancedescriptor : CrudDescriptor<EquipoInstance>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Equipo en inventario");
            OnModuleMenu();

            Property(p => p.Id).Id("Número de serie").AsListColumn();
            ObjectProperty(p => p.Parent).Label("Modelo de equipo").AsListColumn().ReadOnly();
            ObjectProperty(p => p.CustomSpecs).Selectable().Creatable().Nullable().Label("Especificaciones personalizadas");
            TextProperty(p => p.Notes).Big().Nullable().Label("Notas");

            CustomAction("Generar etiqueta de número de serie", BarcodeHelper.MkBarcode);
        }
    }
}