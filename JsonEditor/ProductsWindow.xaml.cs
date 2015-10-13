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
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        MainWindow mParent;
        List<RoomItem> mRooms;

        public ProductsWindow(MainWindow parent, List<RoomItem> rooms, List<ManufacturerItem> manufacturers)
        {
            InitializeComponent();

            mRooms = rooms;
            mParent = parent;

            CBRoom.ItemsSource = mRooms;
            CBRoom.SelectedIndex = 0;

            CBNamufacturer.ItemsSource = manufacturers;
            CBNamufacturer.SelectedIndex = 0;

            if(rooms.Count == 0 || rooms[0].child.Count ==0 || rooms[0].child[0].child.Count == 0 || manufacturers.Count == 0)
            {
                MessageBox.Show("Nemozno pridat produkt pretoze nemas izby alebo vyrobcov");
                BTNAdd.IsEnabled = false;
            }

            var Items = new Dictionary<string, object>();
            var names = Enum.GetNames(typeof(ObjectPlacementOptions));
            var options = Enum.GetValues(typeof(ObjectPlacementOptions)).Cast<ObjectPlacementOptions>().ToArray();
            for (int i = 1; i < names.Length; i++)
            {
                Items.Add(names[i], options[i]);
            }

            var SelectedItems = new Dictionary<string, object>();


            MC.ItemsSource = Items;
            MC.SelectedItems = SelectedItems;
        }

        private void BTNAdd_Click(object sender, RoutedEventArgs e)
        {
            int owerwriteIdx = -1;
            InteriorProduct alreadyAdded = mRooms[CBRoom.SelectedIndex].child[CBType.SelectedIndex].child[CBSubType.SelectedIndex].products.FirstOrDefault(x => x.uidisplayname == TBXMeno.Text);
            if(alreadyAdded != null)
            {
                MessageBoxResult result = MessageBox.Show(this, "Vybrany subtyp uz obsahuje tento produkt. Prepisat ?", "Prepisat produkt ?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                switch(result)
                {
                    case MessageBoxResult.Yes:
                        owerwriteIdx = mRooms[CBRoom.SelectedIndex].child[CBType.SelectedIndex].child[CBSubType.SelectedIndex].products.IndexOf(alreadyAdded);
                        break;
                    case MessageBoxResult.No:
                        return;
                        break;
                }
            }
            InteriorProduct product = new InteriorProduct();
            product.uidisplayname = TBXMeno.Text;
            product.path = TBXPathBundle.Text;
            product.image = TBXPathImage.Text;
            product.description = TBXShortDescription.Text;
            product.descriptionfordetail = TBXLongDescription.Text;
            product.pathformanufacturer = TBXPathManufacturer.Text;
            var products = mRooms[CBRoom.SelectedIndex].child[CBType.SelectedIndex].child[CBSubType.SelectedIndex].products;
            product.id = products.Count == 0 ? 0 : products.Max(x => x.id + 1);
            product.room = CBRoom.SelectedIndex;
            product.typ = CBType.SelectedIndex;
            InteriorObjectSubtype selectedSubTyp = CBSubType.SelectedItem as InteriorObjectSubtype;
            product.subtyp = selectedSubTyp.category == UIElementCategory.Undefined ? -1 : selectedSubTyp.id;
            product.manufacturerid = CBNamufacturer.SelectedIndex;
            product.manufacturername = (CBNamufacturer.SelectedItem as ManufacturerItem).uidisplayname;
            product.placementOptions = ObjectPlacementOptions.None;
            product.hash = TBXHash.Text;

            if (!string.IsNullOrEmpty(product.hash))
            {
                uint crc;
                if (!uint.TryParse(TBXCrc.Text, out crc))
                {
                    MessageBox.Show("Crc field is empty or has incorrect value", "Invalid product");
                    return;
                }
                else
                    product.crc = crc;
            }

            foreach (var sel in MC.SelectedItems)
            {
                product.placementOptions |= (ObjectPlacementOptions)sel.Value;
            }

            if (owerwriteIdx != -1)
                mRooms[CBRoom.SelectedIndex].child[CBType.SelectedIndex].child[CBSubType.SelectedIndex].products[owerwriteIdx] = product;
            else
                mRooms[CBRoom.SelectedIndex].child[CBType.SelectedIndex].child[CBSubType.SelectedIndex].products.Add(product);
        }

        private void CBRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<InteriorObjectType> availableTypes = new List<InteriorObjectType>();
            foreach(var type in mRooms[CBRoom.SelectedIndex].child)
            {
                availableTypes.Add(type);
            }

            CBType.SelectedIndex = 0;
            CBType.ItemsSource = availableTypes;
        }

        private void CBType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<InteriorObjectSubtype> availableSubTypes = new List<InteriorObjectSubtype>();
            if (CBType.SelectedIndex < 0)
                CBType.SelectedIndex = 0;
            foreach (var type in mRooms[CBRoom.SelectedIndex].child[CBType.SelectedIndex].child)
            {
                availableSubTypes.Add(type);
            }

            CBSubType.SelectedIndex = 0;
            CBSubType.ItemsSource = availableSubTypes;
        }

        private void CBSubType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void CBNamufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BTNAddManufacturer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNAddToomSubtype_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNAddRoomType_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BTNAddRoom_Click(object sender, RoutedEventArgs e)
        {
            WindowWithStringResult addWindow = new WindowWithStringResult(
                (name) =>
                {
                    RoomItem newRoom = new RoomItem();
                    newRoom.uidisplayname = name;
                    newRoom.id = mRooms.Max(x => x.id) + 1;
                    mRooms.Add(newRoom);

                    List<RoomItem> availableRooms = new List<RoomItem>(mRooms);

                    CBRoom.ItemsSource = availableRooms;
                    CBRoom.SelectedIndex = availableRooms.Count - 1;
                });
            addWindow.Show();
        }

        private void BTNAddClose_Click(object sender, RoutedEventArgs e)
        {
            BTNAdd_Click(null, null);
            Close();
        }
    }
}
