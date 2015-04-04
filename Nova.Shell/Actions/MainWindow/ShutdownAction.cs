using System.Linq;
using System.Windows;
using Nova.Library.Actions;
using RESX = Nova.Shell.Properties.Resources;

namespace Nova.Shell.Actions.MainWindow
{
    public class ShutdownAction : LeaveAction<MainView, MainViewModel>
    {
        private bool _isLoading;

        public override void OnBefore()
        {
            _isLoading = ViewModel.Sessions.Any(x => x.IsLoading);
        }

        public override bool Leave()
        {
            if (!_isLoading) return true;
            
            var dialog = MessageBox.Show(RESX.CloseApplication, RESX.CloseApplicationTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);

            return dialog == MessageBoxResult.Yes;
        }
        public override void LeaveCompleted()
        {
            Application.Current.Shutdown();
        }
    }
}
