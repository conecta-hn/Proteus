/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component.Attributes;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Seeders
{
    /// <summary>
    /// Semillador que genera roles de usuario predefinidos.
    /// </summary>
    [Order(1), SeederFor(typeof(UserService))]
    public class RolesSeeder : AsyncDbSeeder<UserRole>
    {
        /// <summary>
        /// Genera las nuevas entidades a insertar en la base de datos.
        /// </summary>
        /// <returns>
        /// Una enumeración de las nuevas entidades a insertar en la base de 
        /// datos.
        /// </returns>
        protected override IEnumerable<UserRole> GenerateEntities()
        {
            return new[] {
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    ButtonBehavior = Models.Base.SecurityBehavior.Unlocked,
                    ModuleBehavior = Models.Base.SecurityBehavior.Unlocked,
                    DefaultGranted = SecurityFlags.Root,
                    Name = "Superusuarios",
                },
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    ButtonBehavior = Models.Base.SecurityBehavior.Unlocked,
                    ModuleBehavior = Models.Base.SecurityBehavior.Unlocked,
                    DefaultGranted = SecurityFlags.Admin,
                    Name = "Admninistradores de configuración",
                },
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    ButtonBehavior = Models.Base.SecurityBehavior.Unlocked,
                    ModuleBehavior = Models.Base.SecurityBehavior.Unlocked,
                    DefaultGranted = SecurityFlags.FullAdmin,
                    Name = "Admninistradores completos",
                },
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    ButtonBehavior = Models.Base.SecurityBehavior.Visible,
                    ModuleBehavior = Models.Base.SecurityBehavior.Visible,
                    DefaultGranted = SecurityFlags.Search,
                    Name = "Usuarios estándar",
                },
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    DefaultGranted = SecurityFlags.Elevate,
                    Name = "Usuarios elevables",
                },
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    ButtonBehavior = Models.Base.SecurityBehavior.Locked,
                    ModuleBehavior = Models.Base.SecurityBehavior.Locked,
                    DefaultGranted = SecurityFlags.None,
                    DefaultRevoked = SecurityFlags.None,
                    Name = "Usuarios restringidos",
                },
            };
        }
    }
}