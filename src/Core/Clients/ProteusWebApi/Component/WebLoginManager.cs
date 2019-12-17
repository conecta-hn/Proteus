/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace TheXDS.Proteus.Component
{
    public class WebLoginManager : ILoginSource
    {
        public Task<LoginResult> Login(string user, SecureString password)
        {
            throw new NotImplementedException();
        }
    }
}