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
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Pages.Base;

namespace TheXDS.Proteus.FacturacionUi.Pages
{
    /// <summary>
    /// Lógica de interacción para InventarioMovePage.xaml
    /// </summary>
    public partial class InventarioMovePage : ProteusPage
    {
        public InventarioMovePage()
        {
            InitializeComponent();
            ViewModel = new InventarioMoveViewModel(this);

        }
    }
}
