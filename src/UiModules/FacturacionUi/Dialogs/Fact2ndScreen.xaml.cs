/*
Copyright © 2017-2019 César Andrés Morgan
Desarrollado bajo el auspicio de Grupo Sinergia S. de R. L. como un ERP para
Laboratorios Médicos, S. de R. L.. Propiedad intelectual de César Andrés Morgan
Licenciado para uso interno solamente.
===============================================================================
Este archivo forma parte de SLM. Su uso debe limitarse a la implementación de
herramientas internas para Laboratorios Médicos S. de R. L. El contenido de
este archivo no debe ser distribuido en ningún producto comercial ni debe ser
reutilizado con otros fines no relacionados. El autor se absuelve de toda
responsabilidad y daños causados por el uso indebido de este archivo o de
cualquier parte de su contenido.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Api;
using System.Windows;
using static TheXDS.Proteus.Misc.AppInternal;

namespace TheXDS.Proteus.Dialogs
{
    /// <summary>
    ///     Lógica de interacción para Fact2ndScreen.xaml
    /// </summary>
    public partial class Fact2ndScreen : ICloseable
    {
        public Fact2ndScreen()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ToScreen(FacturaService.GetEstation.SecondScreen ?? 1);
        }
    }
}
