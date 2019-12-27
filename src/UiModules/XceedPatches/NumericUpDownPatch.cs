/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Plugins;
using System;
using TheXDS.MCART.Attributes;
using Xceed.Wpf.Toolkit;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace XceedPatches
{
    /// <summary>
    ///     Aplica parches a objetos <see cref="NumericUpDown{T}"/>.
    /// </summary>
    [Description("Aplica parches a objetos NumericUpdown<T> de Xceed Wpf Toolkit 3.5.0")]
    public class NumericUpDownPatch : Patch
    {
        /// <summary>
        ///     Aplica un parche personalizado a un control 
        ///     <see cref="NumericUpDown{T}"/>.
        /// </summary>
        /// <param name="o">
        ///     Control al cual aplicar el parche.
        /// </param>
        public override void Apply(object o)
        {
            var p = o.GetType().GetProperty("FormatString")!;
            if (p.GetValue(o) is null)
            {
                p.SetValue(o, string.Empty);
            }
        }

        /// <summary>
        ///     Comprueba que este parche pueda ser aplicado al objeto 
        ///     especificado.
        /// </summary>
        /// <param name="type">
        ///     Tipo de objeto a comprobar.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si este parche puede ser aplicado al
        ///     objeto del tipo especificado, <see langword="false"/> en caso
        ///     contrario.
        /// </returns>
        public override bool Patches(Type type)
        {
            return type.Implements(typeof(CommonNumericUpDown<>));
        }
    }
}
