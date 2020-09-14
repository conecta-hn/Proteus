/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    public class RootElevator : IElevator
    {
        public bool Elevate(ref IProteusUserCredential credential)
        {
            credential = Proteus.ResolveLink<User>("root") ?? new User
            {
                AllowMultiLogin = true,
                Interactive = false,
                ScheduledPasswordChange = false,
                DefaultGranted=SecurityFlags.Root,
                DefaultRevoked=SecurityFlags.None,
                ButtonBehavior=SecurityBehavior.Unlocked,
                ModuleBehavior=SecurityBehavior.Unlocked,
                Id = System.Environment.UserName,
                Name = $"{System.Environment.UserName} en {System.Environment.UserDomainName}",
            };
            return true;
        }
    }
}