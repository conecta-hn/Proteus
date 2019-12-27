/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.ViewModels;
using System;
using System.Windows.Controls;

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    /// Interaction logic for ListEditor.xaml
    /// </summary>
    public partial class ListEditor : UserControl
    {
        private ListEditorViewModel Vm => (ListEditorViewModel)DataContext;
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ListEditor"/>.
        /// </summary>
        public ListEditor()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vm.AddSelections = (sender as ListView).SelectedItems;
        }
        internal void ClearSelection(object sender, EventArgs e)
        {
            LstSelection.UnselectAll();
        }
    }
}
