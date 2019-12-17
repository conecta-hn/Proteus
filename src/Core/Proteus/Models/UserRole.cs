/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class UserRole : ProteusCredential, IProteusRoleCredential, IProteusRwRoleCredential
    {
        public virtual List<ProteusHierachicalCredential> Members { get; set; } = new List<ProteusHierachicalCredential>();
        IEnumerable<IProteusCredential> IProteusRoleCredential.Members => Members.AsReadOnly();
        ICollection<ProteusHierachicalCredential> IProteusRwRoleCredential.Members => Members;
    }
}