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
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;

namespace TheXDS.Proteus.Tools
{
    [Name("Herramienta de actualización")]
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
            if (asm is null || asm.IsDynamic) return null;
            var p = asm.GetName();

            return !(p.Name is null || asm.Location.Contains("C:\\Program Files\\dotnet\\shared"))
                ? new AsmInfo($"{Path.GetFileName(p.CodeBase)}", p.Version?.ToString() ?? "1.0.0.0")
                : (AsmInfo?)null;
        }

        public override Task PostLoadAsync()
        {
            if (!Settings.Default.UpdateCheck) return Task.CompletedTask;
            _updtTimer = new Timer(TimeSpan.FromMinutes(Settings.Default.UpdateInterval).TotalMilliseconds);
            _updtTimer.Elapsed += UpdtTimer_Elapsed;
            return base.PostLoadAsync();
        }

        private async void UpdtTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!Settings.Default.UpdateCheck)
            {
                _updtTimer?.Stop();
                _updtTimer = null;
                return;
            }
            await PostLoginAsync();
        }

        public override async Task PostLoginAsync()
        {
            if (!Settings.Default.UpdateCheck) return;
            if (_updtTimer is null) await PostLoadAsync();
            var list = AppDomain.CurrentDomain.GetAssemblies().Select(GetInfo).NotNull().ToList();
            var request = WebRequest.Create($"{Settings.Default.UpdateServer.TrimEnd('/')}/v1/Update/Check");
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
                        sb.AppendLine($"· {j.name, -30}{j.version, 20}");
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
                Process.Start(updater, $"\"{loc.Replace(".dll", ".exe")}\" \"{Settings.Default.UpdateServer.TrimEnd('/')}\"");
            }
        }
    }
}