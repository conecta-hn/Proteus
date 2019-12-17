/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Linq;

namespace TheXDS.Proteus.Widgets
{
    public struct FileFilter
    {
        public string Description { get; }
        public IEnumerable<string> Extensions { get; }

        public FileFilter(string extension): this($"Archivo {extension}", extension)
        {
        }

        public FileFilter(string description, string extension)
        {
            Description = description;
            Extensions = new HashSet<string> { $"*.{extension}" };
        }

        public FileFilter(string description, IEnumerable<string> extensions)
        {
            Description = description;
            Extensions = new HashSet<string>(extensions.Select(p => $"*.{p}"));
        }
    }
}