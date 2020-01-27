/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;

namespace TheXDS.Proteus.Models.Base
{
    public interface IProteusHierachicalParentCredential : IProteusHierachicalCredential
    {
        IEnumerable<IProteusHierachicalCredential> Children { get; }
    }
}