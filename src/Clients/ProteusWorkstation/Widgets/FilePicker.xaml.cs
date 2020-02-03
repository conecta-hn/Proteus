/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Controls;

namespace TheXDS.Proteus.Widgets
{

    /// <inheritdoc cref="UserControl"/>
    /// <summary>
    /// Lógica de interacción para FilePicker.xaml
    /// </summary>
    public partial class FilePicker : PathPickerBase
    {

        public FilePicker()
        {
            InitializeComponent();
            InitializeComponent2();
        }

        protected override TextBox PathBox => Txt;
        protected override Button BrowseButton => BtnOpen;
    }
}