/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus.Tools
{

    public class UpdateCheckTool : Tool
    {
        private struct AsmInfo
        {
            public AsmInfo(string name, string version)
            {
                this.name = name;
                this.version = version;
            }

            public string name { get; set; }
            public string version { get; set; }
        }

        private Timer? _updtTimer;

        private AsmInfo? GetInfo(Assembly? asm)
        {
            if (asm is null) return null;
            var p = asm.GetName();

            // Comprobar la longitud de InformationalVersion ayuda a filtrar ensamblados de .Net Core.
            var v = asm.GetAttr<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return !(p.Name is null ||  v?.Length > 20)
                ? new AsmInfo($"{Path.GetFileName(p.CodeBase)}{Path.GetExtension(p.CodeBase)}", v ?? p.Version?.ToString() ?? "1.0.0.0")
                : (AsmInfo?)null;
        }

        public override Task PostLoadAsync()
        {
            _updtTimer = new Timer(TimeSpan.FromMinutes(15).TotalMilliseconds);
            _updtTimer.Elapsed += UpdtTimer_Elapsed;
            return base.PostLoadAsync();
        }

        private async void UpdtTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await PostLoginAsync();
        }

        public override async Task PostLoginAsync()
        {
            var list = AppDomain.CurrentDomain.GetAssemblies().Select(GetInfo).NotNull().ToList();
            foreach (var j in list.ToArray())
            {
                if (File.Exists(j.name.Replace(".dll", ".exe")))
                {
                    list.Add(new AsmInfo
                    {
                        name = j.name.Replace(".dll", ".exe"),
                        version = j.version ?? App.Info.Version?.ToString() ?? "1.0.0.0"
                    });
                }
            }

            var request = WebRequest.Create($"https://localhost:44363/v1/Update/Check");
            request.Method = "POST";
            request.ContentType = "application/json";

            await JsonSerializer.SerializeAsync(await request.GetRequestStreamAsync(), list.ToArray());
            try
            {
                var resp = (await request.GetResponseAsync()).GetResponseStream();
                var updates = await JsonSerializer.DeserializeAsync<AsmInfo[]>(resp);
                if (updates.Any())
                {
                    using var fw = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "proteusUpdate.lock"), FileMode.Create);
                    using var bw = new BinaryWriter(fw);
                    bw.Write(updates.Length);
                    var sb = new StringBuilder();
                    sb.AppendLine("Estas son las actualizaciones disponibles:");
                    foreach (var j in updates)
                    {
                        bw.Write(j.name);
                        bw.Write(j.version);
                        sb.AppendLine($"· {j.name,-60} {j.version,30}");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"Reinicie {App.Info.Name} para aplicar las actualizaciones.");
                    App.BeforeClose.Add(OnApplyUpdates);
                    Proteus.AlertTarget?.Alert($"Hay {updates.Length} actualizaciones disponibles", sb.ToString(), CloseApp);
                }
            }
            catch { }
        }

        private void CloseApp(Alerta obj)
        {
            obj.Dismiss();
            App.Close();
        }

        private void OnApplyUpdates()
        {
            var loc = typeof(App).Assembly.Location;            
            foreach (var f in  new DirectoryInfo(Path.GetDirectoryName(loc)).GetFiles("ProteusUpdater.*"))
            {
                f.CopyTo(Path.Combine(Path.GetTempPath(), f.Name), true);
            }
            var updater = Path.Combine(Path.GetTempPath(), "ProteusUpdater.exe");
            if (File.Exists(updater))
            {
                Process.Start(updater, $"\"{loc.Replace(".dll", ".exe")}\"");
            }
        }
    }
}