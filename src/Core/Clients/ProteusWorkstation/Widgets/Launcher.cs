/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Windows.Input;
using TheXDS.MCART;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    ///     Describe a un objeto que representa un acceso desde la UI a la
    ///     funcionalidad deseada.
    /// </summary>
    public class Launcher : INameable, IDescriptible
    {
        /// <summary>
        ///     Obtiene un nombre para este <see cref="Launcher"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Obtiene una descripción para este <see cref="Launcher"/>.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        ///     Obtiene un comando para este <see cref="Launcher"/>.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        ///     Obtiene un Id de identficación del método de este launcher.
        /// </summary>
        public string MethodId { get; }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="Launcher"/>.
        /// </summary>
        /// <param name="name">
        ///     Nombre a mostrar del <see cref="Launcher"/>.
        /// </param>
        /// <param name="description">
        ///     Descripción del <see cref="Launcher"/>.
        /// </param>
        /// <param name="id">
        ///     Identificador del método a ejecutar por este
        ///     <see cref="Launcher"/>.
        /// </param>
        /// <param name="command">
        ///     Comando a ejecutar al activar este <see cref="Launcher"/>.
        /// </param>
        public Launcher(string name, string? description, string id, ICommand command) : this(name, description, id, command, null) { }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="Launcher"/>.
        /// </summary>
        /// <param name="name">
        ///     Nombre a mostrar del <see cref="Launcher"/>.
        ///     </param>
        /// <param name="description">
        ///     Descripción del <see cref="Launcher"/>.
        /// </param>
        /// <param name="action">
        ///     Acción a ejecutar al activar este <see cref="Launcher"/>.
        /// </param>
        public Launcher(string name, string? description, Action action) : this(name, description, action.Method.FullName(), new SimpleCommand(action), null)
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="Launcher"/>.
        /// </summary>
        /// <param name="name">
        ///     Nombre a mostrar del <see cref="Launcher"/>.
        ///     </param>
        /// <param name="description">
        ///     Descripción del <see cref="Launcher"/>.
        /// </param>
        /// <param name="id">
        ///     Identificador del método a ejecutar por este
        ///     <see cref="Launcher"/>.
        /// </param>
        /// <param name="command">
        ///     Comando a ejecutar al activar este <see cref="Launcher"/>.
        /// </param>
        /// <param name="commandParameter">
        ///     Parámetros del comando.
        /// </param>
        public Launcher(string name, string? description, string id, ICommand command, object? commandParameter)
        {
            Name = name;
            Description = description;
            Command = command;
            CommandParameter = commandParameter;
            MethodId = id;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="Launcher"/>.
        /// </summary>
        /// <param name="name">
        ///     Nombre a mostrar del <see cref="Launcher"/>.
        ///     </param>
        /// <param name="command">
        ///     Comando a ejecutar al activar este <see cref="Launcher"/>.
        /// </param>
        /// <param name="id">
        ///     Identificador del método a ejecutar por este
        ///     <see cref="Launcher"/>.
        /// </param>
        public Launcher(string name, ICommand command, string id) : this(name, null, id, command, null) { }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="Launcher"/>.
        /// </summary>
        /// <param name="name">
        ///     Nombre a mostrar del <see cref="Launcher"/>.
        ///     </param>
        /// <param name="command">
        ///     Comando a ejecutar al activar este <see cref="Launcher"/>.
        /// </param>
        /// <param name="commandParameter">
        ///     Parámetros del comando.
        /// </param>
        /// <param name="id">
        ///     Identificador del método a ejecutar por este
        ///     <see cref="Launcher"/>.
        /// </param>
        public Launcher(string name, ICommand command, object commandParameter,string id) : this(name, null, id, command, commandParameter) { }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="Launcher"/>.
        /// </summary>
        /// <param name="command">
        ///     Comando a ejecutar al activar este <see cref="Launcher"/>.
        /// </param>
        /// <param name="id">
        ///     Identificador del método a ejecutar por este
        ///     <see cref="Launcher"/>.
        /// </param>
        public Launcher(ICommand command, string id) : this("👆", null,id, command) { }
   
        /// <summary>
        ///     Obtiene el parámetro a utilizar al realizar llamadas a los
        ///     métodos del comando.
        /// </summary>
        public object? CommandParameter { get; }

        /// <summary>
        ///     Convierte implícitamente un <see cref="InteractionItem"/> en un <see cref="Launcher"/>.
        /// </summary>
        /// <param name="j">
        ///     <see cref="InteractionItem"/> a convertir.
        /// </param>
        public static implicit operator Launcher(InteractionItem j)
        {
            return new Launcher(
                j.Text,
                j.Description,
                j.Action.Method.FullName(),
                new SimpleCommand(() => j.Action(j, EventArgs.Empty)));
        }
    }
}