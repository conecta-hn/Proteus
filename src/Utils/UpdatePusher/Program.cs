using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UpdatePusher
{
    class Program
    {
        private static FileStream? GetManifest(FileMode mode)
        {
            try
            {
                return new FileStream(Path.GetFullPath("..\\release.manifest"), mode);
            }
            catch { return null; }
        }

        static void Main(string[] args)
        {
            using var fw = GetManifest(FileMode.Create);
            if (fw is null) return;
            using var sw = new BinaryWriter(fw);

            foreach (var j in Directory.GetFiles(".", "*.dll"))
            {
                try
                {
                    Assembly.LoadFrom(j);
                }
                catch { }
            }

            var list = AppDomain.CurrentDomain.GetAssemblies()
                .Where(p => p != typeof(Program).Assembly)
                .Where(p => !p.Location.Contains("C:\\Program Files\\dotnet\\shared"))
                .Select(FilterOut)
                .Where(p => !(p is null))
                .Cast<(string location, string version)>()
                .ToList();

            sw.Write(list.Count);
            foreach (var (location, version) in list)
            {
                sw.Write(location);
                sw.Write(version);
            }
        }

        private static (string location, string version)? FilterOut(Assembly j)
        {
            try
            {
                var i = j.GetName().Version?.ToString() ?? "1.0.0.0";
                //var i = j.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? j.GetName().Version?.ToString() ?? "1.0.0.0";
                return (Path.GetFileName(j.Location), i);
            }
            catch
            {
                Console.WriteLine($"Neles pasteles con {j.Location} {j.GetName().Version}");
                return null;
            }
        }
    }
}
