using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        List<InteriorProduct> allProducts;
        int currentIdx = 0;
        Dictionary<string, object> Items;

        public ProductEditWindow(List<RoomItem> rooms)
        {
            InitializeComponent();

            allProducts = new List<InteriorProduct>();
            for (int i = 0; i < rooms.Count; i++) //izby
            {
                for (int j = 0; j < rooms[i].child.Count; j++) //Typ
                {
                    for (int k = 0; k < rooms[i].child[j].child.Count; k++) //Typ Typ
                    {
                        for (int l = 0; l < rooms[i].child[j].child[k].products.Count; l++)
                        {
                            InteriorProduct product = rooms[i].child[j].child[k].products[l];
                            allProducts.Add(product);
                        }
                    }
                }
            }
            MainGrid.DataContext = allProducts[currentIdx];

            Items = new Dictionary<string, object>();
            var names = Enum.GetNames(typeof(ObjectPlacementOptions));
            var options = Enum.GetValues(typeof(ObjectPlacementOptions)).Cast<ObjectPlacementOptions>().ToArray();
            for (int i = 1; i < names.Length; i++)
            {
                Items.Add(names[i], options[i]);
            }

            var SelectedItems = new Dictionary<string, object>();


            MC.ItemsSource = Items;
            MC.SelectedItems = SelectedItems;

            ChangeComboBoxSelection(allProducts[currentIdx]);
        }

        private void BTNNext_Click(object sender, RoutedEventArgs e)
        {
            ChangeProductPlacementOptions();

            currentIdx++;
            if (currentIdx == allProducts.Count)
                currentIdx = 0;
            MainGrid.DataContext = allProducts[currentIdx];

            ChangeComboBoxSelection(allProducts[currentIdx]);
        }

        private void BTNPrev_Click(object sender, RoutedEventArgs e)
        {
            ChangeProductPlacementOptions();

            currentIdx--;
            if (currentIdx < 0)
                currentIdx = allProducts.Count - 1;
            MainGrid.DataContext = allProducts[currentIdx];

            ChangeComboBoxSelection(allProducts[currentIdx]);
        }

        private void ChangeProductPlacementOptions()
        {
            InteriorProduct pr = allProducts[currentIdx];
            pr.placementOptions = ObjectPlacementOptions.None;
            foreach(var sel in MC.SelectedItems)
            {
                pr.placementOptions |= (ObjectPlacementOptions)sel.Value;
            }
        }
        private void ChangeComboBoxSelection(InteriorProduct pr)
        {
            var newSelection = new Dictionary<string, object>();
            var names = Enum.GetNames(typeof(ObjectPlacementOptions));
            var options = Enum.GetValues(typeof(ObjectPlacementOptions)).Cast<ObjectPlacementOptions>().ToArray();
            for (int i = 1; i < names.Length; i++)
            {
                if ((options[i] & pr.placementOptions) == options[i])
                {
                    newSelection.Add(names[i], options[i]);
                }
            }

            MC.ItemsSource = new Dictionary<string, object>();
            MC.ItemsSource = Items;
            MC.SelectedItems = newSelection;
            //(MC.DataContext as ViewModel).SelectedItems = new Dictionary<string, object>();
        }
    }
}
