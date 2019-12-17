using System.Security;
using System.Threading.Tasks;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    ///     Describe una serie de miembros a implementar por una clase que
    ///     permita comprobar credenciales de inicio de sesión para propósitos
    ///     varios.
    /// </summary>
    public interface ILoginSource
    {
        /// <summary>
        ///     Comprueba las credenciales brindadas para realizar un inicio de
        ///     sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        ///     Un objeto <see cref="LoginResult"/> que describe el resultado
        ///     de la operación.
        /// </returns>
        Task<LoginResult> Login(string user, SecureString password);
    }
}