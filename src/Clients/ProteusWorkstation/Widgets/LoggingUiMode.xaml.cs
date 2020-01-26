/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Controls;
using System.Windows.Documents;
using TheXDS.Proteus.ViewModels;

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    /// Lógica de interacción para LoggingUiMode.xaml
    /// </summary>
    public partial class LoggingUiMode : UserControl
    {
        public LoggingUiMode()
        {
            InitializeComponent();
        }

        public LoggingUiMode(MainWindowViewModel vm) : this()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            lock (TxtLog)
            {
                if (e.PropertyName == nameof(MainWindowViewModel.Status))
                {
                    if (!(_lastLog is null))
                    {
                        App.UiInvoke(() => TxtLog.Inlines.Add(new LineBreak()));
                        App.UiInvoke(() => TxtLog.Inlines.Add(new Run(_lastLog)));
                    }
                    _lastLog = ((MainWindowViewModel)sender).Status;
                }
            }
        }
        private string? _lastLog;
    }
}