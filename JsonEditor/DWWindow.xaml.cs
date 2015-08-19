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
    /// Interaction logic for DWWindow.xaml
    /// </summary>
    public partial class DWWindow : Window
    {
        MainWindow mParent;
        ObservableCollection<DWItem> observableDoors;
        ObservableCollection<DWItem> observableWindows;

        public DWWindow(MainWindow parent, DW doors, DW windows)
        {
            InitializeComponent();

            observableDoors = new ObservableCollection<DWItem>(doors.child);
            observableWindows = new ObservableCollection<DWItem>(windows.child);
            mParent = parent;

            ItemsDoors.ItemsSource = observableDoors;
            ItemsWindows.ItemsSource = observableWindows;
        }

        private void DoorButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string buttonText = b.Content as string;
            if (buttonText == "Edit")
            {
                Grid sp = b.DataContext as Grid;
                foreach (var child in sp.Children)
                {
                    if (!(child is TextBox))
                        continue;
                    (child as TextBox).IsEnabled = true;
                }
                b.Content = "OK";
            }
            else
            {
                Grid sp = b.DataContext as Grid;
                foreach (var child in sp.Children)
                {
                    if (!(child is TextBox))
                        continue;
                    (child as TextBox).IsEnabled = false;
                }
                b.Content = "Edit";
            }
        }

        private void DoorButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            DWItem item = (sender as Button).DataContext as DWItem;
            observableDoors.Remove(item);
        }

        private void ButtonAddDoor_Click(object sender, RoutedEventArgs e)
        {
            DWItem door = new DWItem();
            door.nazov = "New door";
            door.asset = "";
            door.image = "";
            observableDoors.Insert(0, door);
        }

        private void ButtonAddWindow_Click(object sender, RoutedEventArgs e)
        {
            DWItem door = new DWItem();
            door.nazov = "New window";
            door.asset = "";
            door.image = "";
            observableWindows.Insert(0, door);
        }

        private void WindowButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string buttonText = b.Content as string;
            if (buttonText == "Edit")
            {
                StackPanel sp = b.DataContext as StackPanel;
                foreach (var child in sp.Children)
                {
                    if (!(child is TextBox))
                        continue;
                    (child as TextBox).IsEnabled = true;
                }
                b.Content = "OK";
            }
            else
            {
                StackPanel sp = b.DataContext as StackPanel;
                foreach (var child in sp.Children)
                {
                    if (!(child is TextBox))
                        continue;
                    (child as TextBox).IsEnabled = false;
                }
                b.Content = "Edit";
            }
        }

        private void WindowButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            DWItem item = (sender as Button).DataContext as DWItem;
            observableWindows.Remove(item);
        }

        private void ButtonSaveClose_Click(object sender, RoutedEventArgs e)
        {
            List<DWItem> doors = observableDoors.ToList();
            List<DWItem> windows = observableWindows.ToList();

            bool isValid = ValidateDW(doors) && ValidateDW(windows);
            if (!isValid)
                return;

            mParent.UpdateDW(doors, windows);
            Close();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool ValidateDW(List<DWItem> dw)
        {
            for(int i = dw.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(dw[i].nazov) || string.IsNullOrEmpty(dw[i].asset) || string.IsNullOrEmpty(dw[i].image))
                {
                    MessageBoxResult result = MessageBox.Show(this, "Okno alebo dvere nemaju vyplnene vsetky udaje - pri polozke cislo " + i + ". Chcete upravit polozku ?", "Chybna polozka", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes, MessageBoxOptions.None);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            return false;
                            break;
                        case MessageBoxResult.No:
                            dw.RemoveAt(i);
                            continue;
                            break;
                        default:
                            dw.RemoveAt(i);
                            continue;
                            break;
                    }
                }
            }

            return true;
        }
    }
}
