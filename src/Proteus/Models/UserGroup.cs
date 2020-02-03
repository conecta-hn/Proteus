/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;

namespace TheXDS.Proteus.Models
{
    public class UserGroup : ProteusHierachicalCredential, IProteusHierachicalParentCredential, IProteusRwHierachicalParentCredential
    {
        public virtual List<ProteusHierachicalCredential> Children { get; set; } = new List<ProteusHierachicalCredential>();
        IEnumerable<IProteusHierachicalCredential> IProteusHierachicalParentCredential.Children => Children.AsReadOnly();
        ICollection<ProteusHierachicalCredential> IProteusRwHierachicalParentCredential.Children => Children;
    }
}
