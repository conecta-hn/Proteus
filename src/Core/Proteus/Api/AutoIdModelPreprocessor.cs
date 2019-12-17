/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Data.Entity.Infrastructure;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    ///     Clase base que permite establecer automáticamente el Id de una
    ///     entidad cuando Entity Framework no sería capaz de determinarlo.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de campo llave para el cual este
    ///     <see cref="AutoIdModelPreprocessor{T}"/> aplica.
    /// </typeparam>
    public abstract class AutoIdModelPreprocessor<T> : IModelPreprocessor where T : IComparable<T>
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
        public bool CanProcess(DbEntityEntry entry)
        {
            var entity = entry.Entity as ModelBase;
            return entity.IdType == typeof(T) && !entity.HasId;
        }

        /// <summary>
        ///     Procesa una entidad.
        /// </summary>
        /// <param name="entry">Entidad a procesar.</param>
        public void Process(DbEntityEntry entry)
        {
            var entity = entry.Entity as ModelBase;
            entity.GetType().GetProperty("Id",typeof(T)).SetValue(entity,GetNewValue());
        }

        /// <summary>
        ///     Obtiene un nuevo valor de campo llave para esta entidad.
        /// </summary>
        /// <returns>
        ///     Un nuevo valor a utilizar como valor del campo llave de la entidad.
        /// </returns>
        protected abstract T GetNewValue();
    }
}