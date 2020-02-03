/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.ViewModels;

namespace TheXDS.Proteus.Pages
{
    /// <summary>
    /// Lógica de interacción para GenericReportPage.xaml
    /// </summary>
    public partial class GenericReportPage : ProteusPage
    {
        public GenericReportPage()
        {
            InitializeComponent();
            ViewModel = new GenericReportViewModel(this);
        }
    }
}
