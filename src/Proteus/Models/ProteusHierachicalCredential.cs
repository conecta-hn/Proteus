/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public abstract class ProteusHierachicalCredential : ProteusCredential, IProteusHierachicalCredential, IProteusRwHierachicalCredential
    {
        public virtual UserGroup Parent { get; set; }
        public virtual List<UserRole> Roles { get; set; } = new List<UserRole>();
        IProteusHierachicalParentCredential IProteusHierachicalCredential.Parent => Parent;
        IEnumerable<IProteusRoleCredential> IProteusHierachicalCredential.Roles => Roles.AsReadOnly();
        ICollection<UserRole> IProteusRwHierachicalCredential.Roles => Roles;
    }
}