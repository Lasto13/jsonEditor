using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JsonEditor
{
    /// <summary>
    /// Interaction logic for WindowWithStringResult.xaml
    /// </summary>
    public partial class WindowWithStringResult : Window
    {
        Action<string> onResultCallback;

        public WindowWithStringResult(Action<string> onResult)
        {
            InitializeComponent();

            onResultCallback = onResult;
        }

        private void OKClicked_Click(object sender, RoutedEventArgs e)
        {
            onResultCallback(TBXResult.Text);
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                OKClicked_Click(null, null);
            }
        }
    }
}
