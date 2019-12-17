/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Crud.Base
{
    public interface IPropertyTextDescriptor : IPropertyDescriptor
    {
        /// <summary>
        ///     Indica que un campo de texto debe ser grande.
        /// </summary>
        /// <returns>
        ///     Una referencia a la misma instancia para utilizar sintáxis
        ///     Fluent.
        /// </returns>
        IPropertyTextDescriptor Big();
        IPropertyTextDescriptor TextKind(TextKind kind);
        IPropertyTextDescriptor MinLength(int minLength);
        IPropertyTextDescriptor MaxLength(int maxLength);
        IPropertyTextDescriptor Mask(string mask);
    }
}