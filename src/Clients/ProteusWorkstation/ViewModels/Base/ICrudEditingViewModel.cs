/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TheXDS.Proteus.Crud;

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

        /// <summary>
        /// Obtiene un <see cref="CrudElement"/> con información sobre los
        /// componentes relacionados al modelo de datos de la entidad
        /// seleccionada.
        /// </summary>
        CrudElement? SelectedElement { get; }

        /// <summary>
        /// Obtiene o establece al elemento seleccionado.
        /// </summary>
        object? Selection { get; set; }

        /// <summary>
        /// Ejecuta una operación colocando a este 
        /// <see cref="ICrudEditingViewModel"/> en estado de ocupado.
        /// </summary>
        /// <param name="action">Tarea a ejecutar.</param>
        void BusyDo(Task action);
    }
}