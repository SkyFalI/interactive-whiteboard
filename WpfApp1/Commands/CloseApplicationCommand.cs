using System.Windows;
using WpfApp1.Commands.Base;

namespace WpfApp1.Commands
{
    public class CloseApplicationCommand : Command
    {
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter) => Application.Current.Shutdown();
    }
}
