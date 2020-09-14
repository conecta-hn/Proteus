/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using TheXDS.MCART.Resources;

namespace TheXDS.Proteus.Resources
{
    /// <summary>
    /// Contiene íconos genéricos que pueden ser utilizados en cualquier parte de Proteus.
    /// </summary>
    public static class Images
    {
        private static readonly StringUnpacker _unpacker = new StringUnpacker(typeof(Images).Assembly, typeof(Images).FullName!);

        /// <summary>
        /// Obtiene una nueva instancia del logo de Laboratorios Médicos sin Texto.
        /// </summary>
        public static UIElement Logo => GetXamlIcon();

        /// <summary>
        /// Obtiene una nueva instancia del ícomo a utilizar para
        /// representar a Proteus.
        /// </summary>
        public static UIElement Proteus => GetXamlIcon();

        /// <summary>
        /// Obtiene una nueva instancia del ícomo a utilizar para
        /// representar a un plugin.
        /// </summary>
        public static UIElement Plugin => GetXamlIcon();

        /// <summary>
        /// Obtiene una nueva instancia del logo de advertencia.
        /// </summary>
        public static UIElement Warning => GetXamlIcon();

        private static UIElement GetXamlIcon([CallerMemberName] string id = null!)
        {
            using var sr = new StringReader(_unpacker.Unpack($"{id}.xaml", new NullGetter()));
            var xx = XmlReader.Create(sr);
            try
            {
                return XamlReader.Load(xx) as UIElement ?? WpfIcons.BadFile;
            }
            catch
            {
                return WpfIcons.FileMissing;
            }
        }
    }
}