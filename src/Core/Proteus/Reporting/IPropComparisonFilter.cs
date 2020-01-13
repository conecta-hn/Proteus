/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    /// Define una serie de miembros a implementar por un 
    /// <see cref="IFilter"/> que utilice una comparación entre una
    /// propiedad de una enttidad de datos y un valor constante.
    /// </summary>
    public interface IPropComparisonFilter : IFilter
    {
        /// <summary>
        /// Obtiene o establece la propiedad del modelo para la cual se
        /// crearán las expresiones.
        /// </summary>
        System.Reflection.PropertyInfo Property { get; set; }

        /// <summary>
        /// Obtiene o establece el valor constante que se utilizará para
        /// comparar contra el valor de la propiedad.
        /// </summary>
        string Value { get; set; }
    }
}