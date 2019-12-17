/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    ///     Enumera los posibles valores de resultado de inicio de sesión.
    /// </summary>
    public enum LoginResultCode : byte
    {
        [Name("Se ha iniciado sesión correctamente.")] Ok,
        [Name("Usuario requerido.")] NoUser,
        [Name("Usuario desconocido.")] UnknownUser,
        [Name("Tipo de usuario inválido. se requiere un usuario interactivo.")] NotInteractive,
        [Name("Tipo de usuario inválido. se requiere un usuario de servicio.")] NotSvcUser,
        [Name("Usuario deshabilitado.")] DisabledUser,
        [Name("El usuario no tiene una contraseña establecida.")] NoPassword,
        [Name("Contraseña inválida.")] InvalidPassword,
        [Name("El equipo no está configurado para iniciar sesión automáticamente.")] StationNotConfigured,
        [Name("Token de inicio de sesión inválido.")] InvalidToken,
        [Name("El token de inicio de sesión ha expirado.")] ExpiredToken,
        [Name("No se ha inciado sesión")] NotLoggedIn,
        [Name("No se pudo iniciar sesión.")] DoNotExplain = byte.MaxValue - 1,
        [Name("Error desconocido iniciando sesión.")] Unknown = byte.MaxValue,
    }
}