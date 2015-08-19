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
    /// Interaction logic for RoomManagementWindow.xaml
    /// </summary>
    public partial class RoomManagementWindow : Window
    {
        MainWindow mParent;
        JsonBase json;

        public RoomManagementWindow(MainWindow parent, JsonBase json)
        {
            InitializeComponent();

            mParent = parent;
            this.json = json;

            tvRooms.ItemTemplateSelector = new A();
            tvRooms.ItemsSource = new List<JsonBase>() { json };
        }
        private void UpdateTree()
        {
            tvRooms.ItemsSource = null;
            tvRooms.ItemsSource = new List<JsonBase>() { json };
        }

        private void BTNAddRoom_Click(object sender, RoutedEventArgs e)
        {
            WindowWithStringResult w = new WindowWithStringResult(
                (name) =>
                    {
                        RoomItem room = new RoomItem();
                        room.uidisplayname = name;
                        room.id = json.elements.Count > 0 ? json.elements.Max(x => x.id) + 1 : 0;
                        json.elements.Add(room);

                        UpdateTree();
                    });
            w.Owner = this;
            w.ShowDialog();
        }

        private void BTNAddRoomType_Click(object sender, RoutedEventArgs e)
        {
            WindowWithStringResult w = new WindowWithStringResult(
                (name) =>
                {
                    RoomItem room = (sender as Button).DataContext as RoomItem;

                    InteriorObjectType typ = new InteriorObjectType();
                    typ.uidisplayname = name;
                    typ.id = room.child.Count > 0 ? room.child.Max(x => x.id) + 1 : 0;
                    room.child.Add(typ);

                    UpdateTree();
                });
            w.Owner = this;
            w.ShowDialog();
        }

        private void BTNAddRoomSubtype_Click(object sender, RoutedEventArgs e)
        {
            WindowWithStringResult w = new WindowWithStringResult(
                (name) =>
                {
                    InteriorObjectType typ = (sender as Button).DataContext as InteriorObjectType;

                    if (typ.child.Count == 1 && typ.child[0].category == UIElementCategory.Undefined)
                    {
                        var products = typ.child[0].products;
                        InteriorObjectSubtype subTyp = new InteriorObjectSubtype(string.IsNullOrEmpty(name) ? UIElementCategory.Undefined : UIElementCategory.ProductTypeType);
                        subTyp.products = products;
                        subTyp.uidisplayname = name;
                        subTyp.id = 0;
                        subTyp.parentid = typ.id;

                        typ.child[0] = subTyp;
                    }
                    else
                    {

                        InteriorObjectSubtype subTyp = new InteriorObjectSubtype(string.IsNullOrEmpty(name) ? UIElementCategory.Undefined : UIElementCategory.ProductTypeType);
                        subTyp.uidisplayname = name;
                        subTyp.parentid = typ.id;
                        subTyp.id = typ.child.Count > 0 ? typ.child.Max(x => x.id) + 1 : 0;

                        typ.child.Add(subTyp);
                    }

                    UpdateTree();
                });
            w.Owner = this;
            w.ShowDialog();
        }

        private void BTNSaveClose_Click(object sender, RoutedEventArgs e)
        {
            if(IsValidJson(json))
                Close();
        }
        private bool IsValidJson(JsonBase json)
        {
            for (int i = json.elements.Count - 1; i >= 0; i-- )
            {
                if(!IsValidRoom(json.elements[i]))
                {
                    MessageBoxResult mResult = MessageBox.Show(this, string.Format("Izba \"{0}\" nieje vytvorena spravne. Chcete tuto izbu vymazat ?", json.elements[i].uidisplayname), "Nespravna izba", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes);
                    switch(mResult)
                    {
                        case MessageBoxResult.Yes:
                            json.elements.RemoveAt(i);
                            UpdateTree();
                            break;
                        case MessageBoxResult.No:
                            return false;
                            break;
                    }
                }
            }
            return true;
        }
        private bool IsValidRoom(RoomItem room)
        {
            if (room.child.Count == 0)
                return false;
            foreach(var typ in room.child)
            {
                if (typ.child.Count == 0)
                    return false;
            }
            return true;
        }

        private void BTNRemoveRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomItem room = (sender as Button).DataContext as RoomItem;
            int roomIdx = json.elements.IndexOf(room);
            for (int i = roomIdx + 1; i < json.elements.Count; i++ )
            {
                RoomItem r = json.elements[i];
                r.id--;
                for(int j = 0; j < r.child.Count; j++)
                {
                    InteriorObjectType t = r.child[j];
                    for(int k = 0; k < t.child.Count; k++)
                    {
                        InteriorObjectSubtype st = t.child[k];
                        for(int l = 0; l < st.products.Count; l++)
                        {
                            st.products[l].room = r.id;
                        }
                    }
                }
            }

            json.elements.RemoveAt(roomIdx);
            UpdateTree();
        }

        private void BTNRenameRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomItem room = (sender as Button).DataContext as RoomItem;
            WindowWithStringResult w = new WindowWithStringResult((name) =>
            {
                room.uidisplayname = name;
                UpdateTree();
            });
            w.ShowDialog();
        }

        private void BTNRemoveType_Click(object sender, RoutedEventArgs e)
        {
            InteriorObjectType type = (sender as Button).DataContext as InteriorObjectType;

            RoomItem parentRoom = null;
            foreach(var room in json.elements)
            {
                if(room.child.Contains(type))
                {
                    parentRoom = room;
                    break;
                }
            }
            if(parentRoom != null)
                parentRoom.child.Remove(type);

            UpdateTree();
        }

        private void BTNRenameType_Click(object sender, RoutedEventArgs e)
        {
            InteriorObjectType room = (sender as Button).DataContext as InteriorObjectType;
            WindowWithStringResult w = new WindowWithStringResult((name) =>
            {
                room.uidisplayname = name;
                UpdateTree();
            });
            w.ShowDialog();
        }

        private void BTNRemoveSubType_Click(object sender, RoutedEventArgs e)
        {
            InteriorObjectSubtype subType = (sender as Button).DataContext as InteriorObjectSubtype;
            InteriorObjectType parentType = null;
            foreach(var room in json.elements)
            {
                foreach(var type in room.child)
                {
                    if(type.child.Contains(subType))
                    {
                        parentType = type;
                        break;
                    }
                }
            }
            if (parentType != null)
                parentType.child.Remove(subType);

            UpdateTree();
        }

        private void BTNRenameSubType_Click(object sender, RoutedEventArgs e)
        {
            InteriorObjectSubtype room = (sender as Button).DataContext as InteriorObjectSubtype;
            WindowWithStringResult w = new WindowWithStringResult((name) =>
            {
                room.uidisplayname = name;
                room.category = string.IsNullOrEmpty(name) ? UIElementCategory.Undefined : UIElementCategory.ProductTypeType;
                UpdateTree();
            });
            w.ShowDialog();
        }
    }

    class A : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is InteriorProduct)
            {
                var enumerator = container.GetLocalValueEnumerator();
                enumerator.MoveNext();
                enumerator.MoveNext();
                enumerator.MoveNext();
                enumerator.MoveNext();
                return enumerator.Current.Value as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
