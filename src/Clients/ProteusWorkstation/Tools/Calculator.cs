/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Plugins;
using System;
using System.Diagnostics;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Tools
{
    [Name("Utilerías varias")]
    public class Calculator : Tool
    {
        [InteractionItem]
        [Name("%")]
        [Description("Abre la calculadora.")]
        public void OpenCalc(object sender, EventArgs e)
        {
            Process.Start("calc");
        }
    }
}