using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Define una serie d emiembros a implementar por un tipo que permita
    /// escribir entradas de bitácora que describan acciones ejecutadas sobre
    /// una base de datos.
    /// </summary>
    public interface IDbLogger
    {
        /// <summary>
        /// Escribe una entrada de bitácora que describe los cambios realizados
        /// en la base de datos.
        /// </summary>
        /// <param name="ct">Objeto que contiene información sobre los cambios
        /// en la base de datos.
        /// </param>
        /// <param name="credential">
        /// Credencial del usuario ejecutando la acción. Puede establecerse en
        /// <see langword="null"/> para implementaciones sin usuario, o para
        /// acciones que no son realizadas por un usuario.
        /// </param>
        /// <returns>
        /// El resultado de la operación de escritura en la bitácora.
        /// </returns>
        Task<Result> Log(DbChangeTracker ct, IProteusCredential? credential);
    }
}