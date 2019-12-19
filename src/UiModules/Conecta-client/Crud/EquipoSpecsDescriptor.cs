/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class EquipoSpecsDescriptor : CrudDescriptor<EquipoSpecs>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Especificaciones de equipo");

            ObjectProperty(p => p.Cpu).Selectable().Creatable().Nullable().Label("CPU");
            NumericProperty(p => p.Ram).Range(1, 16777216).Label("Memoria RAM (en MiB)").Default(4096);
            ObjectProperty(p => p.Gpu).Selectable().Creatable().Nullable().Label("Tarjeta de video");
            Property(p => p.Wifi).Label("Tiene WiFi");
            Property(p => p.Bluetooth).Label("Tiene Bluetooth");
            Property(p => p.OpticalDrive).Nullable().Label("Unidad óptica");
            NumericProperty(p => p.Usb2).Range(0, 100).Label("Cantidad de puertos USB 2.0").Default(4);
            NumericProperty(p => p.Usb3).Range(0, 100).Label("Cantidad de puertos USB 3.0");
            Property(p => p.TypeC).Label("Tiene puertos USB Type-C");
            Property(p => p.Vga).Label("Salidas VGA");
            Property(p => p.Dvi).Label("Salidas DVI");
            Property(p => p.Hdmi).Label("Salidas HDMI");
            Property(p => p.DisplayPort).Label("Salidas DisplayPort");
        }
    }
}