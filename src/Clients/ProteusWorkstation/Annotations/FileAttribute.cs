/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Component.Attributes
{
    /// <summary>
    /// Indica que un elemento utilizará un filtro de archivo personalizado
    /// a la hora de generar un cuadro de diálogo para abrir o guardar
    /// archivos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FileAttribute : TextAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FileAttribute" />.
        /// </summary>
        /// <param name="fileFilter">Filtro de archivos a aplicar.</param>
        public FileAttribute(string fileFilter) : base(fileFilter)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="T:TheXDS.Proteus.Component.Attributes.FileAttribute" />.
        /// </summary>
        public FileAttribute():base("Todos los archivos|*")
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FileAttribute" />.
        /// </summary>
        /// <param name="values">Tipos de archivo admitidos.</param>
        public FileAttribute(IEnumerable<KeyValuePair<string, string>> values) : base(string.Join("|", values.SelectMany(Decompose)))
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FileAttribute" />.
        /// </summary>
        /// <param name="values">Tipos de archivo admitidos.</param>
        public FileAttribute(IEnumerable<string> values) : this(values.Select(p =>
            new KeyValuePair<string, string>($"Archivo {p}", $"*.{p}")))
        {

        }

        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="FileAttribute" />.
        /// </summary>
        /// <param name="values">Tipos de archivo admitidos.</param>
        public FileAttribute(params (string, string)[] values) : base(string.Join("|", values.SelectMany(Decompose)))
        {
            
        }

        /// <summary>
        /// Crea un nuevo diálogo de apertura de archivos.
        /// </summary>
        /// <returns></returns>
        public OpenFileDialog MakeDialog()
        {
            return new OpenFileDialog
            {
                Title = "Seleccionar archivo",
                Filter = Value,
                CheckFileExists = true,
                Multiselect = false,
                DereferenceLinks = true,
                CheckPathExists = true,
            };
        }

        private static IEnumerable<string> Decompose(KeyValuePair<string, string> pair)
        {
            yield return pair.Key;
            yield return pair.Value;
        }
        private static IEnumerable<string> Decompose((string, string) tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
        }
    }
}