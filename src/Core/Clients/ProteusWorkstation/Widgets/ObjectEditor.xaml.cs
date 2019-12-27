/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheXDS.Proteus.ViewModels;
using System;
using System.Windows.Controls;

namespace TheXDS.Proteus.Widgets
{

    /// <summary>
    /// Lógica de interacción para ObjectEditor.xaml
    /// </summary>
    public partial class ObjectEditor : UserControl
    {
        private ObjectEditorViewModel Vm => (ObjectEditorViewModel)DataContext;

        public ObjectEditor()
        {
            InitializeComponent();
        }
    }
}
