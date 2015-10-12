using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JsonEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string message = "Unhandled exception occured:\n" + e.Exception.Message + "\nStacktrace:\n" + e.Exception.StackTrace;
            string caption = "Oooooops, nedobre :/";
            MessageBox.Show(message , caption, MessageBoxButton.OK);
            e.Handled = true;
        }
    }
}
