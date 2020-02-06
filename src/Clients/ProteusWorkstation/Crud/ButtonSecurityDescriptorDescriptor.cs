/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Linq;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Describe un descriptor de seguridad para un <see cref="Launcher"/>.
    /// </summary>
    public class ButtonSecurityDescriptorDescriptor : SecurityDescriptorDescriptor<ButtonSecurityDescriptor, Launcher>
    {
        /// <inheritdoc/>
        protected override void DescribeModel()
        {
            FriendlyName("Descriptor de botón");
            base.DescribeModel();
            Property(p => p.Visible).Important("Visible");
            Property(p => p.Available).Important("Disponible");
            Property(p => p.Elevatable).Important("Elevable");
        }

        protected override string GetApi(Launcher selection)
        {
            return selection.MethodId;
        }

        /// <inheritdoc/>
        protected override IEnumerable<Launcher> Source()
        {
            return App.Modules.SelectMany(p => p.FullMenu).SelectMany(p => p.Value);
        }
    }
}