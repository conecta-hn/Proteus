/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Devel
{
    public abstract class MenuOption : Plugin
    {
        protected AdminSessionClient Client => ProteusDevel.NwClient;
        public abstract string Summary { get; }
        internal abstract void Run();
        protected string Input(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"{prompt}: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            return Console.ReadLine();
        }
        protected ICollection<string> InputList(string prompt)
        {
            var inputs = new List<string>();
            InputList(prompt, inputs);
            return inputs;
        }
        protected void InputList(string prompt, ICollection<string> inputs)
        {
            Console.WriteLine(prompt);
            string input;
            do
            {
                input = Input($"(Vacío para finalizar, {inputs.Count} elemento(s) hasta ahora)");
                if (!input.IsEmpty()) inputs.Add(input);
            } while (!input.IsEmpty());

        }
        protected void Separator() => Separator('-');
        protected void Separator(char sepChar)
        {
            Console.WriteLine(new string(sepChar, Console.BufferWidth));
        }
    }
}