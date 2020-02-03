/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Controls;
using System.Windows.Input;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.Widgets
{

    /// <summary>
    /// Lógica de interacción para ObjectEditor.xaml
    /// </summary>
    public partial class ObjectEditor : UserControl
    {
        public ObjectEditor()
        {
            InitializeComponent();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var vm = (ISearchViewModel)DataContext;
            if (Keyboard.Modifiers != ModifierKeys.None) return;
            switch (e.Key)
            {
                case Key.Enter:
                    vm.SearchCommand.TryExecute();
                    ((TextBox)sender).SelectAll();
                    break;
                case Key.Escape:
                    vm.ClearSearch();
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }
    }
}
