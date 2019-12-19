/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Inventario.Models
{
    public class EquipoDefinition : Nameable<string>
    {
        /// <summary>
        ///     Colección de los proveedores que bridan este item.
        /// </summary>
        public virtual List<Proveedor> ProvistoPor { get; set; } = new List<Proveedor>();

        /// <summary>
        ///     Cajas, lotes, Cardex o batches que representan las existencias
        ///     en bulk dentro del inventario.
        /// </summary>
        public virtual List<Batch> Batches { get; set; } = new List<Batch>();

        public EquipoSpecs? DefaultSpecs { get; set; }        
    }

    public class EquipoInstance : ModelBase<string>
    {
        public EquipoDefinition Parent { get; set; }
        public EquipoSpecs? CustomSpecs { get; set; }

        public string? Notes { get; set; }

        public override string ToString()
        {
            return $"{Parent}{Id.OrNull(" ({0})")}";
        }
    }

    public class EquipoSpecs : ModelBase<Guid>
    {
        public CpuInventory? Cpu { get; set; }
        public int Ram { get; set; }
        public GpuInventory? Gpu { get; set; }
        public bool Wifi { get; set; }
        public bool Bluetooth { get; set; }
        public bool TypeC { get; set; }

        public OpticalDriveType? OpticalDrive { get; set; }
        public byte Usb2 { get; set; }
        public byte Usb3 { get; set; }
        public byte Vga { get; set; }
        public byte Dvi { get; set; }
        public byte Hdmi { get; set; }
        public byte DisplayPort { get; set; }

        public override string ToString()
        {
            return $"{Cpu?.ToString() ?? "Sin procesador"}, {Ram} MiB RAM, {Gpu?.ToString() ?? "video integrado"}";
        }
    }

    public enum OpticalDriveType : byte
    {
        [Name("Lectora de CD")] CdRom,
        [Name("Quemadora de CD")] CdRw,
        [Name("Lectora de DVD")] DvdRom,
        [Name("Quemadora de DVD")] DvdRw,
        [Name("Lectora de BD")] BdRom,
        [Name("Quemadora de BD")] BdR
    }

    public enum CpuMaker : byte
    {
        Intel,
        [Name("AMD")] Amd,
        [Name("VIA")] Via,
        Arm
    }

    public enum GpuMaker : byte
    {
        [Name("nVidia")] Nvidia,
        [Name("AMD")] Amd,
        Intel
    }

    public class CpuInventory : Nameable<int>
    {
        public CpuMaker Maker { get; set; }
    }

    public class GpuInventory : Nameable<int>
    {
        public GpuMaker Maker { get; set; }
    }
}