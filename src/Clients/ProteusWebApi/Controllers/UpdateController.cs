/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.AspNetCore.Mvc;
using TheXDS.Proteus.Controllers.Base;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;

namespace TheXDS.Proteus.Controllers
{
    public class UpdateController : ProteusWebController
    {
        private static FileStream? GetManifest(FileMode mode)
        {
            try
            {
                return new FileStream(Path.Combine(Startup.Settings!.DataDir, "release.manifest"), mode);
            }
            catch { return null; }
        }

        [HttpPost]
        public ActionResult<AsmInfo[]> Check([FromBody]AsmInfo[] input)
        {
            var updts = new List<AsmInfo>();
            using var fw = GetManifest(FileMode.Open);
            if (fw is null) return Array.Empty<AsmInfo>();
            using var sr = new BinaryReader(fw);

            var t = sr.ReadInt32();

            for (var i = 0; i < t; i++)
            {
                var name = sr.ReadString();
                var version = sr.ReadString();

                if (input.FirstOrDefault(p=>p.name == name) is { } a)
                {
                    if (version.CompareTo(a.version) == 1)
                    {
                        updts.Add(new AsmInfo(name, version));
                    }
                }
            }

            return updts.ToArray();
        }
        [HttpPost]
        public ActionResult WriteManifest([FromBody]AsmInfo[] input)
        {
            using var fw = GetManifest(FileMode.Create);
            if (fw is null) return new StatusCodeResult(500);

            using var sw = new BinaryWriter(fw);
            sw.Write(input.Length);
            foreach(var j in input)
            {
                sw.Write(j.name);
                sw.Write(j.version);
            }

            return new OkResult();
        }

        [HttpPost]
        public ActionResult DownloadFile([FromBody]string assemblyId)
        {
            try
            {
                return new FileStreamResult(new FileStream(Path.Combine(Startup.Settings!.DataDir, "release", assemblyId), FileMode.Open), "application/octect-stream");
            }
            catch
            {
                return new StatusCodeResult(404);
            }
        }
    }
}