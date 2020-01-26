/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Clase que administra grupos cerrables de páginas.
    /// </summary>
    public class CloseableGroup : ICloseable
    {
        private readonly HashSet<ICloseable> _closeables = new HashSet<ICloseable>();

        /// <summary>
        /// Cierra todos los objetos contenidos en este grupo.
        /// </summary>
        public void Close()
        {
            foreach (var j in _closeables) j.Close();
        }

        /// <summary>
        /// Obtiene una colección de objetos cerrables administrados por
        /// este grupo.
        /// </summary>
        public ICollection<ICloseable> Closeables => _closeables;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CloseableGroup"/>.
        /// </summary>
        public CloseableGroup()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="CloseableGroup"/>.
        /// </summary>
        /// <param name="closeables">Objetos cerrables.</param>
        public CloseableGroup(IEnumerable<ICloseable> closeables)
        {
            foreach (var j in closeables) _closeables.Add(j);
        }
    }
}