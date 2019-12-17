using System;
using System.Collections.Generic;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component.Attributes;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Seeders
{

    [Order(1),SeederFor(typeof(UserService))]
    public class RolesSeeder : AsyncDbSeeder<UserRole>
    {
        protected override IEnumerable<UserRole> GenerateEntities()
        {
            return new[] {
                new UserRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    ButtonBehavior = Models.Base.SecurityBehavior.Unlocked,
                    ModuleBehavior = Models.Base.SecurityBehavior.Unlocked,
                    DefaultGranted = SecurityFlags.Admin,
                    Name = "Admninistradores",
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