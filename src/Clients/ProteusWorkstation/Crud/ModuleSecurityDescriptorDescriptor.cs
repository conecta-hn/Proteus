/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Describe un descriptor de seguridad para un <see cref="UiModule"/>.
    /// </summary>
    public class ModuleSecurityDescriptorDescriptor : SecurityDescriptorDescriptor<ModuleSecurityDescriptor, UiModule>
    {
        /// <inheritdoc/>
        protected override void DescribeModel()
        {
            FriendlyName("Descriptor de módulo");
            base.DescribeModel();
            Property(p => p.Loaded).Important("Cargado");
            Property(p => p.Accesible).Important("Accesible");
        }

        protected override string GetApi(UiModule selection) => selection.GetType().FullName!;

        /// <inheritdoc/>
        protected override IEnumerable<UiModule> Source() => App.Modules;
    }
}