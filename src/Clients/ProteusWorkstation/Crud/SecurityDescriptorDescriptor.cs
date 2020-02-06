/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.ViewModels;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Clase base que permite definir descriptores de Crud para descriptores
    /// de seguridad.
    /// </summary>
    /// <typeparam name="TDescriptor">
    /// Modelo del descriptor de seguridad.
    /// </typeparam>
    /// <typeparam name="TSelection">
    /// Tipo del objeto de seguridad a enlazar.
    /// </typeparam>
    public abstract class SecurityDescriptorDescriptor<TDescriptor, TSelection> : CrudDescriptor<TDescriptor, SecurityDescriptorViewModel<TDescriptor, TSelection>> where TDescriptor : SecurityDescriptor, new() where TSelection : class
    {
        /// <inheritdoc/>
        protected override void DescribeModel()
        {
            VmObjectProperty(p => p.Selection)
                .Source(Source().Select(p => new SecurityObjectSelection<TSelection>(p)).AsQueryable())
                .Required()
                .Important("Objeto de seguridad")
                .Validator(CheckNotNull);

            VmBeforeSave(SetApi);
        }

        private IEnumerable<ValidationError> CheckNotNull(ModelBase arg1, PropertyInfo arg2)
        {
            if (arg1 is null) yield return new NullValidationError(arg2);
        }

        private void SetApi(SecurityDescriptorViewModel<TDescriptor, TSelection> arg1, ModelBase arg2)
        {
            arg1.Entity.Id ??= GetApi(arg1.Selection!.SecurityObject);
        }

        /// <summary>
        /// Enumera los objetos de seguridad a obtener para permitir
        /// seleccionarlos.
        /// </summary>
        /// <returns>
        /// Una enumeración de todos los objetos de seguridad a presentar.
        /// </returns>
        protected abstract IEnumerable<TSelection> Source();
        protected abstract string GetApi(TSelection selection);
    }
}