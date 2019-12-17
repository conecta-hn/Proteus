/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Data.Entity.Infrastructure;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    ///     Describa una serie de miembros a implementar por un tipo que
    ///     ejecute operaciones de preparación de entidades de datos previas al
    ///     guardado.
    /// </summary>
    public interface IModelPreprocessor
    {
        /// <summary>
        ///     Determina si este <see cref="IModelPreprocessor"/> puede
        ///     procesar a la entidad especificada.
        /// </summary>
        /// <param name="entry">Entidad a procesar.</param>
        /// <returns>
        ///     <see langword="true"/> si este <see cref="IModelPreprocessor"/>
        ///     puede procesar a la entidad especificada,
        ///     <see langword="false"/> en caso contrario.
        /// </returns>
        bool CanProcess(DbEntityEntry entry);

        /// <summary>
        ///     Procesa una entidad.
        /// </summary>
        /// <param name="entry">Entidad a procesar.</param>
        void Process(DbEntityEntry entry);
    }
}