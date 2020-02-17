/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TheXDS.Proteus.Updater
{
    public struct AsmInfo
    {
        public AsmInfo(string name, string version)
        {
            this.name = name;
            this.version = version;
        }

        public string name { get; set; }
        public string version { get; set; }

        public static AsmInfo? GetInfo(Assembly? asm)
        {
            if (asm is null) return null;
            var p = asm.GetName();

            // Comprobar la longitud de InformationalVersion ayuda a filtrar ensamblados de .Net Core.
            var v = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return !(p.Name is null || v?.Length > 20)
                ? new AsmInfo($"{p.Name}{Path.GetExtension(p.CodeBase)}", v ?? p.Version?.ToString() ?? "1.0.0.0")
                : (AsmInfo?)null;
        }

        public static implicit operator AsmInfo?(Assembly? asm)
        {
            return GetInfo(asm);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private int _updtCount = 0;
        private int _updtTotal = 0;
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var a = Environment.GetCommandLineArgs();
            string? pth = null;
            string? srv = null;
            if (a.Length == 3)
            {
                pth = a[1];
                srv = a[2];
            }
            try
            {
                var doDelete = true;
                var fname = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "proteusUpdate.lock");
                using (var fw = new FileStream(fname, FileMode.Open))
                {
                    using var br = new BinaryReader(fw);
                    var tot = br.ReadInt32();
                    var list = new List<string>();
                    for(var j = 0; j < tot; j++)
                    {
                        list.Add(br.ReadString());
                        _ = br.ReadString();
                    }

                    PgrProgress.IsIndeterminate = false;
                    _updtTotal = list.Count;
                    foreach (var j in list)
                    {
                        try
                        {
                            var wr = WebRequest.Create($"{srv}/v1/Update/DownloadFile");
                            wr.Timeout = 30000;
                            wr.Method = "POST";
                            wr.ContentType = "application/json";
                            await JsonSerializer.SerializeAsync(await wr.GetRequestStreamAsync(), j);

                            var resp = (HttpWebResponse)await wr.GetResponseAsync();
                            if (resp.StatusCode == HttpStatusCode.OK)
                            {
                                using var fs = new FileStream(pth is { } d ? Path.Combine(Path.GetDirectoryName(d)!, j) : j, FileMode.Create);
                                await Task.WhenAll(resp.GetResponseStream().CopyToAsync(fs), ReportProgress(fs, resp.ContentLength));
                            }
                        }
                        catch (Exception ex)
                        {
                            doDelete = false;
                        }
                        _updtCount++;
                    }
                }
                if (doDelete) File.Delete(fname);
                if (pth is { } p) System.Diagnostics.Process.Start(p);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
            Close();
        }

        private async Task ReportProgress(Stream current, long total)
        {
            while (current.Length < total)
            {
                Dispatcher.Invoke(() => PgrProgress.Value = ((double)_updtCount / _updtTotal * 100.0) + ((double?)current.Length / total * 100.0 / _updtTotal ?? 0.0));
                Thread.Sleep(200);
            }
        }
    }
}
