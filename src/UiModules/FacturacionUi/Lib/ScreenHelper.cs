/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Linq;
using System.Windows;
using WpfScreenHelper;

namespace TheXDS.Proteus.FacturacionUi.Lib
{
    internal static class ScreenHelper
    {
        public static void ToScreen(this Window window, byte screen)
        {
            var sc = Screen.AllScreens.ToList();
            var s = (screen >= sc.Count ? sc.Last() : sc[screen-1]).Bounds;
            window.Top = s.Top;
            window.Left = s.Left;
            window.Width = s.Width;
            window.Height = s.Height;
            window.WindowState = WindowState.Maximized;
        }
    }
}
