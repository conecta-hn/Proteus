/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Seeders;
using System.Collections.Generic;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Facturacion
{
    [Name("Seeder para creación de categorías de clientes.")]
    [SeederFor(typeof(ConectaService))]
    public class ClienteCategorySeeder : AsyncExternalSeeder<ClienteCategory>
    {
        protected override IEnumerable<ClienteCategory> GenerateEntities()
        {
            yield return new ClienteCategory
            {
                Name = "Cliente regular",
                RequireRTN = false,
            };
        }
    }
}
