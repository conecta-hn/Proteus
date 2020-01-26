/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheXDS.MCART;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Reporting;
using System.IO;

namespace TheXDS.Proteus.Misc
{
    public static class Internal
    {
        private static readonly IEnumerable<IModelSearchFilter> _filters = Objects.FindAllObjects<IModelSearchFilter>().ToList();

        public static void Dump(TextWriter tw, Exception? ex, int textWidth = 80)
        {
            InternalDump(tw, ex,textWidth);

            if (ReflectionHelpers.GetEntryPoint() is { } m) PrintInfo(tw, new AssemblyInfo(m.Module.Assembly), "Aplicación cliente");
            if (ex?.TargetSite is { } ts) PrintInfo(tw, new AssemblyInfo(ts.DeclaringType?.Assembly ?? typeof(Internal).Assembly), "Ensamblado con errores");
            tw.WriteLine("Componentes (en orden de carga):");
            foreach (var k in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    PrintInfo(tw, new AssemblyInfo(k));
                }
                catch { }
            }
        }
        
        public static string Dump(this Exception? ex, int textWidth = 80)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))            
                Dump(sw, ex);            

            return sb.ToString();
        }
        
        public static IQueryable Query(string query, Type model)
        {
            var s = query.ToLower();
            var f = new List<IFilter>();
            var o = new OrFilter().PushInto(f);

            foreach (var j in _filters)
            {
                if (j.UsableFor(model)) j.AddFilter(f, o, s);
            }

            return QueryBuilder.BuildQuery(model, f);
        }

        private static void InternalDump(TextWriter j, Exception? ex, int textWidth)
        {
            if (ex is null) return;

            j.WriteLine($"{ex.GetType().Name} en {ex.Source} (0x{ex.HResult.ToString("X").PadLeft(8, '0')})");
            j.WriteLine(new string('-', textWidth));
            foreach (var k in ex.Message.TextWrap(textWidth)) j.WriteLine(k.TrimEnd(' '));
            j.WriteLine(new string('-', textWidth));
            j.WriteLine(ex.StackTrace);
            j.WriteLine(new string('=', textWidth));

            foreach (var k in ex.GetType().GetProperties())
            {
                switch (k.GetValue(ex))
                {
                    case IEnumerable<Exception?> exceptions:
                        foreach (var l in exceptions.NotNull()) InternalDump(j, l, textWidth);
                        break;
                    case Exception inner:
                        InternalDump(j, inner, textWidth);
                        break;
                }
            }
        }
        private static void PrintInfo(TextWriter j, IExposeInfo nfo) => PrintInfo(j, nfo, null);
        private static void PrintInfo(TextWriter j, IExposeInfo nfo, string? name)
        {
            j.WriteLine($"{name.OrNull("{0}: ")}{nfo.Name} {nfo.InformationalVersion}");
        }
    }
}
