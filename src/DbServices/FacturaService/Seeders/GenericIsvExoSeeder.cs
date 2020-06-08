/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Seeders
{
    public class GenericIsvExoSeeder : AsyncDbSeeder<IsvExoneracionType>
    {
        protected override IEnumerable<IsvExoneracionType> GenerateEntities()
        {
            yield return new IsvExoneracionType
            {
                ApplyRule = IsvExoApplyRule.All
            };
        }
    }
}