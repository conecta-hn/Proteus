/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusRwUserCredential : IProteusRwHierachicalCredential
    {
        bool Enabled { get; set; }
        bool ScheduledPasswordChange { get; set; }
        bool AllowMultiLogin { get; set; }
        bool Interactive { get; set; }
    }
}