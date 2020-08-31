/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus.Seeders
{
    /// <summary>
    /// Semillador que genera un cliente genérico de facturación.
    /// </summary>
    [SeederFor(typeof(FacturaService))]
    public class GenericClienteSeeder : AsyncDbSeeder<Cliente>
    {
        protected override IEnumerable<Cliente> GenerateEntities()
        {
            yield return new Cliente
            {
                Name = "Cliente final",
                Timestamp = DateTime.Now,
                Rtn = "9999-9999-999999",
                Category = null,
                Facturas = { },
                Cotizaciones = { },
                Prepaid = 0m,
                Exoneraciones = { },
                CanPrepay = false,
                CanCredit = false
            };
        }
    }
}