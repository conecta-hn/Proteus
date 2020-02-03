/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusUserCredential : IProteusHierachicalCredential
    {
        bool Enabled { get; }
        bool ScheduledPasswordChange { get; }
        bool AllowMultiLogin { get; }
        bool Interactive { get; }
    }
}