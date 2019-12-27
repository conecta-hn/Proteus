using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Reporting;

namespace TheXDS.Proteus.Misc
{
    public static class Internal
    {
        private static readonly IEnumerable<IModelSearchFilter> _filters = Objects.FindAllObjects<IModelSearchFilter>().ToList();

        public static void Dump(StringBuilder j, Exception? ex, int textWidth = 80)
        {
            if (ex is null) return;

            j.AppendLine($"{ex.GetType().Name} en {ex.Source} (0x{ex.HResult.ToString("X").PadLeft(8, '0')})");
            j.AppendLine(new string('-', textWidth));
            foreach (var k in ex.Message.TextWrap(textWidth)) j.AppendLine(k.TrimEnd(' '));
            j.AppendLine(new string('-', textWidth));
            j.AppendLine(ex.StackTrace);
            j.AppendLine(new string('=', textWidth));

            foreach (var k in ex.GetType().GetProperties())
            {
                switch (k.GetValue(ex))
                {
                    case IEnumerable<Exception?> exceptions:
                        foreach (var l in exceptions.NotNull()) Dump(j, l);
                        break;
                    case Exception inner:
                        Dump(j, inner);
                        break;
                }
            }
        }

        public static string Dump(this Exception? ex, int textWidth = 80)
        {
            var sb = new StringBuilder();
            Dump(sb, ex);
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

    }
}
