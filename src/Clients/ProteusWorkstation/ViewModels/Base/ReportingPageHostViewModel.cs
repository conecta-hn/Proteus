using TheXDS.Proteus.Component;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.ViewModels.Base
{
    public abstract class ReportingPageHostViewModel : PageHostViewModel, IStatusReporter
    {
        protected ReportingPageHostViewModel(IPageVisualHost host) : base(host)
        {
        }

        protected ReportingPageHostViewModel(IPageVisualHost host, bool closeable) : base(host, closeable)
        {
        }

        private string? _status;
        private double _progress = double.NaN;
        public double Progress
        {
            get => _progress;
            set
            {
                if (Change(ref _progress, value))
                    Notify(nameof(IndeterminateProgress));
            }
        }
        public string? Status 
        { 
            get => _status;
            private set => Change(ref _status, value);
        }
        public bool IndeterminateProgress => double.IsNaN(_progress);
        public void Done()
        {
            Status = null;
            Progress = double.NaN;
            IsBusy = false;
        }
        public void Done(string text)
        {
            Status = text;
            Progress = double.NaN;
            IsBusy = false;
        }
        public void UpdateStatus(double progress)
        {
            IsBusy = true;
            Status = null;
            Progress = progress;
        }
        public void UpdateStatus(double progress, string text)
        {
            IsBusy = true;
            Progress = progress;
            Status = text;

        }
        public void UpdateStatus(string text)
        {
            IsBusy = true;
            Progress = double.NaN;
            Status = text;
        }
    }
}