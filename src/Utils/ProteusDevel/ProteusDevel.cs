/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART;
using static TheXDS.MCART.Objects;

namespace TheXDS.Proteus.Devel
{
    public static class ProteusDevel
    {
        internal static AdminSessionClient NwClient => (AdminSessionClient)Proteus.NwClient;
        internal static bool _appActive = true;
        private static readonly IList<MenuOption> _options = FindAllObjects<MenuOption>().ToList();

        public static async Task Main(string[] args)
        {
            Proteus.ReplaceClient<AdminSessionClient>();
            await KickStart.Run();

            NwClient.ConnectionLost += (_, e) =>
            {
                Proteus.MessageTarget?.Stop("Se ha perdido la conectividad con el servidor.");
                _appActive = false;
            };

            Console.Write("Usuario: ");
            var user = Console.ReadLine();

            Console.Write("Contraseña: ");
            var pw = new System.Security.SecureString();
            var rpw = true;
            do
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        if (pw.Length > 1)
                            pw.RemoveAt(pw.Length - 1);
                            Console.Write("\b \b");
                        break;
                    case ConsoleKey.Enter:
                        rpw = false;
                        break;
                    default:
                        if (key.KeyChar != '\u0000')
                        {
                            pw.AppendChar(key.KeyChar);
                            Console.Write("*");
                        }
                        break;
                }
            } while (rpw);
            Console.WriteLine();

            Proteus.Interactive = true;
            var r = await Proteus.Login(user, pw);
            if (r.Success)
            {
                while (_appActive)
                {
                    Menu();
                }
                Proteus.Logout();
            }
            else
            {
                Proteus.MessageTarget?.Stop(r.Message);
            }
        }
        private static void Menu()
        {
            PrintMenu();
        }
        private static void PrintMenu()
        {
            var c = 0;
            foreach (var j in _options)
            {
                Console.WriteLine($"{++c}) {j.Summary}");
            }
            Console.Write($"Elija una opción (1-{c}): ");
            int r;
            while (!(int.TryParse(Console.ReadLine(), out r) && r.IsBetween(1, c)))
            {
                Console.WriteLine();
                Console.WriteLine("Opción inválida.");
                Console.Write($"Elija una opción (1-{c}): ");
            }
            Console.WriteLine();
            _options[r - 1].Run();
        }
    }
}