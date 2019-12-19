/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusRwHierachicalParentCredential : IProteusRwHierachicalCredential
    {
        ICollection<ProteusHierachicalCredential> Children { get; }
    }
}