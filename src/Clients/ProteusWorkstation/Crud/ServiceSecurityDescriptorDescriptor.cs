/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.MCART;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Describe un descriptor de seguridad para un <see cref="Service"/>.
    /// </summary>
    public class ServiceSecurityDescriptorDescriptor : SecurityDescriptorDescriptor<ServiceSecurityDescriptor, MethodInfo>
    {
        /// <inheritdoc/>
        protected override void DescribeModel()
        {
            FriendlyName("Descriptor de servicio");
            base.DescribeModel();
            Property(p => p.Granted).Important("Otorgado");
            Property(p => p.Revoked).Important("Denegado");
        }

        protected override string GetApi(MethodInfo selection)
        {
            return selection.FullName();
        }

        /// <inheritdoc/>
        protected override IEnumerable<MethodInfo> Source()
        {
            return Proteus.Services!.SelectMany(p => p.Functions).Select(p => p.Key);
        }
    }
}