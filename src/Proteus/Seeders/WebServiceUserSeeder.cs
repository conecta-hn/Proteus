﻿/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.IO;
using System.Threading.Tasks;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Seeders
{
    /// <summary>
    /// Semillador que genera un token de seguridad predeterminado para el
    /// usuario de API Web.
    /// </summary>
    [SeederFor(typeof(UserService))]
    public class WebServiceUserSeeder : IAsyncDbSeeder
    {
        /// <summary>
        /// Ejecuta la acción de semillado de la base de datos de forma
        /// asíncrona.
        /// </summary>
        /// <param name="service">
        /// Servicio sobre el cual ejecutar las acciones de semillado.
        /// </param>
        /// <param name="reporter">
        /// Objeto que permite reportar el progreso de la operación.
        /// </param>
        /// <returns>
        /// El resultado detallado de la operación de semillado de la base de
        /// datos.
        /// </returns>
        public async Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter? reporter)
        {
            reporter?.UpdateStatus("Creando tokens para usuarios de servicio...");
            var us = (UserService)service;
            var r = await us.GenerateToken("apiwebservice", null, null, out var token);
            var pth = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "apiwebservice.token");
            File.WriteAllText(pth, token);
            reporter?.UpdateStatus(100, $"Token para 'apiwebservice':{Environment.NewLine}{token}{Environment.NewLine} se ha escrito el token en {pth}");
            return r;
        }

        public string GetName => GetType().NameOf();

        public string InformationalVersion
        {
            get
            {
                return new AssemblyInfo(GetType().Assembly).InformationalVersion.OrNull() ?? GetType().Assembly.GetName().Version?.ToString().OrNull() ?? "1.0.0.0";
            }
        }

        /// <summary>
        /// Realiza comprobaciones sobre la base de datos para determinar si
        /// debe ejecutarse o no este semillador.
        /// </summary>
        /// <param name="service">
        /// Servicio sobre el cual ejecutar las acciones de semillado.
        /// </param>
        /// <param name="reporter">
        /// Objeto que permite reportar el progreso de la operación.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si debe ejecutarse el semillado en la base
        /// de datos, <see langword="false"/> en caso contrario.
        /// </returns>
        public async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter? reporter)
        {
            reporter?.UpdateStatus("Comprobando tokens de servicios...");
            return !await service.AnyAsync<LoginToken>(p => p.Login.Id == "apiwebservice");
        }
    }
}