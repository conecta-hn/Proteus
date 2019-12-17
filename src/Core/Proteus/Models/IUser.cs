/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Models.Base
{
    public interface IUser : INameable
    {
        string StringId { get; }
        bool Enabled { get; }
        byte[] PasswordHash { get; }
        bool ScheduledPasswordChange { get; }
    }
}