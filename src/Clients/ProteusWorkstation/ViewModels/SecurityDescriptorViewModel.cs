/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.ViewModel;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Crud;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para las clases derivadas del modelo base
    /// <see cref="SecurityDescriptor"/>.
    /// </summary>
    public class SecurityDescriptorViewModel<TDescriptor, TSelection> : ViewModel<TDescriptor> where TDescriptor : SecurityDescriptor where TSelection : class
    {
        private SecurityObjectSelection<TSelection>? _selection;

        /// <summary>
        ///     Obtiene o establece el valor Selection.
        /// </summary>
        /// <value>El valor de Selection.</value>
        public SecurityObjectSelection<TSelection>? Selection
        {
            get => _selection;
            set => Change(ref _selection, value);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="SecurityDescriptorViewModel{TDescriptor, TSelection}"/>.
        /// </summary>
        public SecurityDescriptorViewModel()
        {
        }
    }
}