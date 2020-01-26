/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Runtime.CompilerServices;
using TheXDS.MCART.Resources;

namespace TheXDS.Proteus.Resources
{
    public static class Art
    {
        public enum Arts
        {
            FullLogo
        }

        private static readonly StringUnpacker _unpacker = new StringUnpacker(typeof(Art).Assembly, typeof(Art).FullName);

        public static string FullLogo => GetArtBase64();

        private static string GetArtBase64([CallerMemberName] string id = null)
        {
            return _unpacker.Unpack($"{id}.png.base64", new DeflateGetter());
        }
    }
}