/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.ComponentModel;
using System.Windows.Input;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// accesar a las funciones específicas de un elemento de Crud en modo
    /// de edición.
    /// </summary>
    public interface ICrudEditingViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Comando que cancela la creación o edición de una entidad.
        /// </summary>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Comando de guardado de entidades, tanto nuevas como editadas.
        /// </summary>
        ICommand SaveCommand { get; }
    }
}