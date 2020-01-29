/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    /// Interaction logic for ListEditor.xaml
    /// </summary>
    public partial class ListEditor : UserControl
    {
        private ListEditorViewModel Vm => (ListEditorViewModel)DataContext;
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ListEditor"/>.
        /// </summary>
        public ListEditor()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            Vm.AddSelections = ((ListView)sender!).SelectedItems;
        }
        internal void ClearSelection(object sender, EventArgs e)
        {
            LstSelection.UnselectAll();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.None) return;
            switch (e.Key)
            {
                case Key.Enter:
                    Vm.SearchCommand.TryExecute();
                    ((TextBox)sender).SelectAll();
                    break;
                case Key.Escape:
                    Vm.ClearSearch();
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }

    }
}
