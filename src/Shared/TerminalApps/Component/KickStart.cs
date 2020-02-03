/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Cmd;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    internal static class KickStart
    {
        private class ProteusOnlyArgument : Argument
        {
            public override string Summary => "Finaliza la aplicación después de inicializar Proteus.";
        }
        private class HelpArgument : HelpArgumentBase
        {
            public override string Summary => "Obtiene la ayuda de este programa.";
        }
        public static async Task<CmdLineParser> Run()
        {
            var args = new CmdLineParser(Environment.GetCommandLineArgs());
            var tgt = new ConsoleMessageTarget();
            Proteus.MessageTarget = tgt;
            Proteus.CommonReporter = tgt;

            args.AutoRun(true);
            if (args.IsPresent<HelpArgument>())
            {
                Environment.Exit(0);
            }

            if (args.Commands.Contains("configure"))
            {
                if (RunConfigure())
                {
                    Settings.Default.Save();
                    Console.WriteLine("Configuración guardada correctamente. Debe reiniciar el servicio para aplicar los cambios.");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Operación cancelada. Se mantendrá la configuración anterior.");
                    Settings.Default.Reload();
                }
            }

            await Proteus.Init();
            if (args.IsPresent<ProteusOnlyArgument>()) Environment.Exit(0);
            return args;
        }
        private static bool RunConfigure()
        {
            static string PrintDefault(PropertyInfo p)
            {
                return p.HasAttr(out DefaultSettingValueAttribute d)
                    ? $" [predeterminado: {d.Value}, actual: {p.GetValue(Settings.Default)}]"
                    : $" [actual: {p.GetValue(Settings.Default)}]";
            }

            static void GetNumber<T>(PropertyInfo p)
            {
                object[] _params = { null, null };
                var m = typeof(T).GetMethod("TryParse", new[] { typeof(string), typeof(T).MakeByRefType() });
                while (!(bool)m.Invoke(null, _params))
                {
                    Console.Write($"{PrintDefault(p)}: ");
                    _params[0] = Console.ReadLine().OrNull() ?? p.GetAttr<DefaultSettingValueAttribute>()?.Value.OrNull() ?? "0";
                }
                p.SetMethod.Invoke(Settings.Default, new object[] { _params[1] });
            }

            foreach (var j in typeof(Settings).GetProperties())
            {
                if (!(j.CanRead && j.CanWrite)) continue;
                if (j.SetMethod.IsStatic) continue;

                //HACK: filtros únicos
                if (j.Name == "SettingsKey") continue;
                if (j.Name.Contains("InitMode") && j.PropertyType != typeof(Proteus.InitMode)) continue;

                Console.Write(j.Name);
                if (j.PropertyType == typeof(bool))
                {
                    char br = default;
                    while (!"SsNn".Contains(br))
                    {
                        Console.Write($" (s/n):{PrintDefault(j)} ");
                        var k = Console.ReadKey();
                        if (k.Key == ConsoleKey.Escape) return false;
                        if (k.Key == ConsoleKey.Enter)
                        {
                            br = (bool.TryParse(j.GetAttr<DefaultSettingValueAttribute>()?.Value, out var b) ? b : default) ? 'S' : 'N';
                        }
                        else br = k.KeyChar;
                    }
                    j.SetMethod.Invoke(Settings.Default, new object[] { "Ss".Contains(br) });
                    Console.WriteLine();
                }
                else if (j.PropertyType == typeof(string))
                {
                    Console.Write($"{PrintDefault(j)}: ");
                    var k = Console.ReadLine();
                    j.SetMethod.Invoke(Settings.Default, new object[] { k.OrNull() ?? j.GetAttr<DefaultSettingValueAttribute>()?.Value.OrNull() ?? string.Empty });
                }
                else if (j.PropertyType == typeof(byte)) GetNumber<byte>(j);
                else if (j.PropertyType == typeof(sbyte)) GetNumber<sbyte>(j);
                else if (j.PropertyType == typeof(short)) GetNumber<short>(j);
                else if (j.PropertyType == typeof(ushort)) GetNumber<ushort>(j);
                else if (j.PropertyType == typeof(int)) GetNumber<int>(j);
                else if (j.PropertyType == typeof(uint)) GetNumber<uint>(j);
                else if (j.PropertyType == typeof(long)) GetNumber<long>(j);
                else if (j.PropertyType == typeof(ulong)) GetNumber<ulong>(j);
                else if (j.PropertyType == typeof(float)) GetNumber<float>(j);
                else if (j.PropertyType == typeof(double)) GetNumber<double>(j);
                else if (j.PropertyType == typeof(decimal)) GetNumber<decimal>(j);
                else if (j.PropertyType.IsEnum)
                {
                    var r = 0;
                    var max = 0;
                    Console.WriteLine();
                    foreach (var k in j.PropertyType.GetEnumNames())
                    {
                        Console.WriteLine($"  {++max}) {k}");
                    }
                    while (!r.IsBetween(1, max))
                    {
                        Console.Write($"Elija un valor (1 - {max}) {PrintDefault(j)}: ");
                        int.TryParse(Console.ReadLine(), out r);
                    }
                    j.SetMethod.Invoke(Settings.Default, new object[] { Enum.GetValues(j.PropertyType).ToGeneric().ToArray()[r - 1] });
                }
                else
                {
                    Console.WriteLine($" <Propiedad {j.PropertyType} no configurable>");
                }
            }
            return true;
        }
    }

    public interface IValueGetter
    {
        bool Gets(Type t);

        void SetValue(object instance, PropertyInfo property);
    }
    public abstract class ValueGetter
    {
        protected string PrintDefault(PropertyInfo p) => PrintDefault(p, out _);
        protected string PrintDefault(PropertyInfo p, out string _default)
        {
            if (p.HasAttr(out DefaultSettingValueAttribute d))
            {
                _default = d.Value;
                return $" [predeterminado: {d.Value}]";
            }
            _default = string.Empty;
            return _default;
        }
    }
    public class NumberValueGetter<T> : ValueGetter, IValueGetter where T : IComparable<T>
    {
        private static readonly MethodInfo _tryParse = typeof(T).NotNullable().GetMethod("TryParse", new[] { typeof(string), typeof(T).MakeByRefType() });

        public bool Gets(Type t) => typeof(T).NotNullable() == t.NotNullable();

        public void SetValue(object instance, PropertyInfo property)
        {
            var _params = new object[2];
            do
            {
                Console.Write($"{ValidRange?.ToString()}{PrintDefault(property, out var _default)}: ");
                _params[0] = Console.ReadLine();
                if (((string)_params[0]).IsEmpty()) _params[0] = _default?.OrNull() ?? ValidRange?.Minimum.ToString();
            } while (!((bool)_tryParse.Invoke(null, _params) && (ValidRange?.IsWithin((T)_params[1]) ?? true)));
            property.SetMethod.Invoke(Settings.Default, new object[] { _params[1] });
        }
        public Range<T>? ValidRange { get; set; }
    }
}