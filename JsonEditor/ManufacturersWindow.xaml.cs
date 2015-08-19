using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ManufacturersWindow.xaml
    /// </summary>
    public partial class ManufacturersWindow : Window
    {
        ObservableCollection<ManufacturerItem> mManufacturers;
        MainWindow mParent;

        private Queue<Action> manufacturersUpdates = new Queue<Action>();

        public ManufacturersWindow(List<ManufacturerItem> manufacturers, MainWindow parent)
        {
            InitializeComponent();

            mManufacturers = new ObservableCollection<ManufacturerItem>(manufacturers);
            ManufacturersList.ItemsSource = mManufacturers;
            mParent = parent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ManufacturerItem manuf = b.DataContext as ManufacturerItem;

            int idx = mManufacturers.IndexOf(manuf);
            manufacturersUpdates.Enqueue(() => mParent.RemoveManufacturer(manuf.id));
            if (idx < mManufacturers.Count - 1)
            {
                ManufacturerItem lastManufacturer = mManufacturers.Last();
                manufacturersUpdates.Enqueue(() =>
                    {
                        mParent.ChangemanufacturerID(lastManufacturer.id, manuf.id);
                    });
                lastManufacturer.id = manuf.id;
            }
            mManufacturers.RemoveAt(idx);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ManufacturerItem manu = new ManufacturerItem();
            manu.id = mManufacturers.Count == 0 ? 0 : mManufacturers.Max(x => x.id) + 1;
            manu.uidisplayname = "New manufacturer";
            mManufacturers.Add(manu);

            manufacturersUpdates.Enqueue(() =>
                {
                    mParent.AddManufacturer(manu);
                });
        }

        private void ButtonSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            List<ManufacturerItem> manufs = mManufacturers.ToList();
            if (!ValidateManufacturers(manufs))
                return;

            mParent.UpdateManufacturers(manufacturersUpdates);
            Close();
        }
        private bool ValidateManufacturers(List<ManufacturerItem> manufacturers)
        {
            for (int i = manufacturers.Count - 1; i >= 0; i-- )
            {
                if(string.IsNullOrEmpty(manufacturers[i].uidisplayname))
                {
                    MessageBoxResult result = MessageBox.Show(this, "Meno vyrobcu je prazdne - pri polozke cislo " + i + ". Chcete tohto vyrobcu vynechat ?", "Chybny vyrobca", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes, MessageBoxOptions.None);
                    switch(result)
                    {
                        case MessageBoxResult.OK:
                            manufacturersUpdates.Enqueue(() =>
                                {
                                    mParent.RemoveManufacturer(mManufacturers[i].id);
                                });
                            mManufacturers.RemoveAt(i);
                            break;
                        case MessageBoxResult.No:
                            return false;
                            break;
                    }
                }
            }
            return true;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
