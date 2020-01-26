/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Input;

namespace TheXDS.Proteus.ViewModels.Base
{
    public interface IEditingCrudViewModel
    {
        ICommand CancelCommand { get; }
        ICommand SaveCommand { get; }
    }
}