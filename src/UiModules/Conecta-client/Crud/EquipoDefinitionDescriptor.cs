/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class EquipoDefinitionDescriptor : CrudDescriptor<EquipoDefinition>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Equipo informático");
            OnModuleMenu();

            Property(p => p.Name).AsName().AsListColumn();
            Property(p => p.Id).Id("Número de modelo");
            ListProperty(p => p.ProvistoPor).Selectable().Required().Label("Proveedores");
            ListProperty(p => p.Batches).Selectable().Creatable().Label("Embarques");
            ObjectProperty(p => p.DefaultSpecs).Selectable().Creatable().Nullable().Label("Especificaciones globales predeterminadas");

            CustomAction("Generar etiqueta de modelo", BarcodeHelper.MkBarcode);

        }
    }
}