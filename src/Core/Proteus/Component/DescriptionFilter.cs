/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    ///     Implementa un <see cref="ModelSearchFilter{T}"/> que puede filtrar
    ///     entidades que implementen la interfaz <see cref="IDescriptible"/>.
    /// </summary>
    public class DescriptionFilter : SimpleModelSearchFilter<IDescriptible>
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="DescriptionFilter"/>.
        /// </summary>
        public DescriptionFilter() : base(nameof(IDescriptible.Description)) { }
    }
}