/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Linq;

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusUserCredential : IProteusHierachicalCredential
    {
        bool Enabled { get; }
        bool ScheduledPasswordChange { get; }
        bool AllowMultiLogin { get; }
        bool Interactive { get; }

        T? GetDescriptor<T>(string id) where T : SecurityDescriptor
        {
            T? ChkCred(IProteusCredential cred)
            {
                foreach (var j in cred.Descriptors.OfType<T>())
                {
                    if (j.Id == id) return j;
                }
                return null;
            }

            IProteusHierachicalCredential c = this;

            while (!(c is null))
            {
                if (ChkCred(c) is { } t) return t;
                foreach (var j in c.Roles)
                {
                    if (ChkCred(j) is { } r) return r;
                }
                c = c.Parent;
            }
            return null;
        }
    }
}