/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Security;
using System.Threading.Tasks;
using TheXDS.Proteus.Api;

namespace TheXDS.Proteus.Component
{
    public class WebLoginManager : ILoginSource
    {
        /// <inheritdoc/>
        public Task<LoginResult> Login(string user, SecureString password)
        {
            throw new NotImplementedException();
        }
    }
}