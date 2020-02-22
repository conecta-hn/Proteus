/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// elevar las credenciales actuales por otras con más permisos.
    /// </summary>
    public interface IElevator
    {
        bool Elevate(ref IProteusUserCredential? credential);
    }
}