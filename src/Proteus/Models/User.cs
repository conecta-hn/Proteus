/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class User : ProteusHierachicalCredential, IProteusUserCredential, IProteusRwUserCredential, IUser
    {
        public byte[] PasswordHash { get; set; }
        public bool Enabled { get; set; } = true;
        public bool ScheduledPasswordChange { get; set; } = true;
        public bool AllowMultiLogin { get; set; }
        public bool Interactive { get; set; } = true;
    }
}