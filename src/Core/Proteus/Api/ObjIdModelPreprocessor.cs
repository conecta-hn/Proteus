/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Data.Entity.Infrastructure;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Procesa los Id de objeto para implementar correctamente las
    /// validaciones que Entity Framework realiza sobre las entidades con
    /// Id de tipo genérico.
    /// </summary>
    /// <remarks>
    /// FIX: Id de ModelBase no se implementa en las bases
    /// de datos, pero es necesario establecerlo para evitar
    /// errores de validación a la hora de guardar.
    /// </remarks>
    [Priority(int.MaxValue)]
    public class ObjIdModelPreprocessor : IModelPreprocessor
    {
        /// <summary>
        /// Determina si este <see cref="IModelPreprocessor"/> puede
        /// procesar a la entidad especificada.
        /// </summary>
        /// <param name="entry">Entidad a procesar.</param>
        /// <returns>
        /// <see langword="true"/> si este <see cref="IModelPreprocessor"/>
        /// puede procesar a la entidad especificada,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public bool CanProcess(DbEntityEntry entry)
        {
            var entity = entry.Entity as ModelBase;
            return entity.Id is null || entity.Id != entity.IdAsObject;
        }

        /// <summary>
        /// Procesa una entidad.
        /// </summary>
        /// <param name="entry">Entidad a procesar.</param>
        public void Process(DbEntityEntry entry)
        {
            var entity = entry.Entity as ModelBase;
            entity.Id = entity.IdAsObject;
        }
    }
}