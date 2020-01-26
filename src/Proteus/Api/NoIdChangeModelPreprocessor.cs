/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Previene el cambio de campos llave primarios a las entidades que ya
    /// han sido guardadas.
    /// </summary>
    public class NoIdChangeModelPreprocessor : IModelPreprocessor
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
            return (entry.State != System.Data.Entity.EntityState.Added)
                   && !entity.IsNew
                   && entity.ChangesPending();
        }

        /// <summary>
        /// Procesa una entidad.
        /// </summary>
        /// <param name="entry">Entidad a procesar.</param>
        public void Process(DbEntityEntry entry)
        {
            var e = entry.Entity as ModelBase;
            var s = Proteus.Infer(e.GetType().ResolveToDefinedType());
            foreach (var j in s.OldValues(e))
            {
                if (!j.Key.HasAttr<KeyAttribute>()) continue;
                j.Key.SetValue(e, j.Value);
            }
        }
    }
}