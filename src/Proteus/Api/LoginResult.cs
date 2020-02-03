/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Estructura que describe el resultado de una operación de inicio
    /// de sesión en Proteus.
    /// </summary>
    public struct LoginResult
    {
        public LoginResult(IUser logon) : this(LoginResultCode.Ok,LoginResultCode.Ok.NameOf(),logon)
        {
        }
        public LoginResult(LoginResultCode result) : this(result, result.NameOf(), null)
        {
        }
        public LoginResult(string message) : this(LoginResultCode.Unknown, message, null) { }
        private LoginResult(LoginResultCode result, string message, IUser logon)
        {
            Result = result;
            Message = message;
            Logon = logon;
        }

        /// <summary>
        /// Indica si la operación fue exitosa o no.
        /// </summary>
        public bool Success => Result == LoginResultCode.Ok;
            
        /// <summary>
        /// Determina el resultado del inicio de sesión utilizando un
        /// código de estado.
        /// </summary>
        public LoginResultCode Result { get; }

        /// <summary>
        /// Mensaje de inicio de sesión.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Referencia a la sesión que fue iniciada.
        /// </summary>
        public IUser Logon { get; }

        /// <summary>
        /// Convierte implícitamente un <see cref="string"/> en un
        /// <see cref="LoginResult"/>.
        /// </summary>
        /// <param name="message">
        /// Objeto a convertir.
        /// </param>
        public static implicit operator LoginResult(string message)
        {
            return new LoginResult(message);
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="bool"/> en un
        /// <see cref="LoginResult"/>.
        /// </summary>
        /// <param name="success">
        /// Objeto a convertir.
        /// </param>
        public static implicit operator LoginResult(bool success)
        {
            return new LoginResult(success ? LoginResultCode.Ok : LoginResultCode.Unknown);
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="LoginResultCode"/> en un
        /// <see cref="LoginResult"/>.
        /// </summary>
        /// <param name="code">
        /// Objeto a convertir.
        /// </param>
        public static implicit operator LoginResult(LoginResultCode code)
        {
            return new LoginResult(code);
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="LoginResult"/> en un
        /// <see cref="bool"/>.
        /// </summary>
        /// <param name="result">
        /// Objeto a convertir.
        /// </param>
        public static implicit operator bool(LoginResult result) => result.Success;

        /// <summary>
        /// Convierte implícitamente un <see cref="LoginResult"/> en un
        /// <see cref="LoginResultCode"/>.
        /// </summary>
        /// <param name="result">
        /// Objeto a convertir.
        /// </param>
        public static implicit operator LoginResultCode(LoginResult result) => result.Result;

        /// <summary>
        /// Convierte implícitamente un <see cref="LoginResult"/> en un
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="result">
        /// Objeto a convertir.
        /// </param>
        public static implicit operator string(LoginResult result) => result.Message;
    }
}