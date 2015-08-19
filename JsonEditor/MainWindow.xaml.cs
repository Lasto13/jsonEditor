using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace JsonEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string baseJson;
        private JsonBase jsonBase;
        private JsonSerializerSettings settings;

        public MainWindow()
        {
            InitializeComponent();

            LabelOpenedFilName.Text = "";

            settings = new JsonSerializerSettings();
            settings.ConstructorHandling = ConstructorHandling.Default;
            settings.DefaultValueHandling = DefaultValueHandling.Include;
            settings.Formatting = Formatting.Indented;
            settings.ObjectCreationHandling = ObjectCreationHandling.Auto;
            settings.TypeNameHandling = TypeNameHandling.None;
            settings.NullValueHandling = NullValueHandling.Ignore;
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON|*.json";
            dialog.FileOk += dialog_FileOk;
            dialog.ShowDialog(this);
        }

        async void dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog d = sender as OpenFileDialog;
            LabelOpenedFilName.Text = d.FileName;
            Stream stream = d.OpenFile();
            StreamReader sr = new StreamReader(stream);
            baseJson = await sr.ReadToEndAsync();

            sr.Close();

            jsonBase = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<JsonBase>(baseJson, settings));

            TXTSkyboxpath.Text = jsonBase.SkyboxPath;
            ButtonSaveFile.IsEnabled = true;
            ButtonManageDW.IsEnabled = true;
            ButtonAddInterior.IsEnabled = true;
            ButtonManageManufacturers.IsEnabled = true;
            ButtonManageRooms.IsEnabled = true;
            BTNEditProducts.IsEnabled = true;
        }

        private void ButtonManageManufacturers_Click(object sender, RoutedEventArgs e)
        {
            ManufacturersWindow manufWin = new ManufacturersWindow(jsonBase.manufacturers, this);
            manufWin.Owner = this;
            manufWin.ShowDialog();
        }

        public void UpdateManufacturers(Queue<Action> updates)
        {
            while(updates.Count > 0)
            {
                updates.Dequeue().Invoke();
            }
        }
        public void UpdateDW(List<DWItem> doors, List<DWItem> windows)
        {
            jsonBase.dvere.pocet = doors.Count;
            jsonBase.okna.pocet = windows.Count;

            jsonBase.dvere.child = doors;
            jsonBase.okna.child = windows;
        }
        
        public void ChangemanufacturerID(int oldID, int newID)
        {
            ManufacturerItem manuf = jsonBase.manufacturers.First(x => x.id == oldID);

            TraverseProducts((product) =>
                {
                    if (product.manufacturerid == oldID)
                        product.manufacturerid = newID;
                });

            manuf.id = newID;
        }
        public void RemoveManufacturer(int manufID)
        {
            ManufacturerItem manuf = jsonBase.manufacturers.First(x => x.id == manufID);

            for (int i = 0; i < jsonBase.elements.Count; i++) //izby
            {
                for (int j = 0; j < jsonBase.elements[i].child.Count; j++) //Typ
                {
                    for (int k = 0; k < jsonBase.elements[i].child[j].child.Count; k++) //Typ Typ
                    {
                        for (int l = jsonBase.elements[i].child[j].child[k].products.Count - 1; l >= 0 ; l--)
                        {
                            if (jsonBase.elements[i].child[j].child[k].products[l].manufacturerid == manufID)
                                jsonBase.elements[i].child[j].child[k].products.RemoveAt(l);
                        }
                    }
                }
            }

            jsonBase.manufacturers.Remove(manuf);
        }
        public void AddManufacturer(ManufacturerItem manuf)
        {
            jsonBase.manufacturers.Add(manuf);
        }
        private void TraverseProducts(Action<InteriorProduct> productAction)
        {
            for (int i = 0; i < jsonBase.elements.Count; i++ ) //izby
            {
                for(int j = 0; j < jsonBase.elements[i].child.Count; j++) //Typ
                {
                    for(int k = 0; k < jsonBase.elements[i].child[j].child.Count; k++) //Typ Typ
                    {
                        for(int l = 0; l < jsonBase.elements[i].child[j].child[k].products.Count; l++)
                        {
                            InteriorProduct product = jsonBase.elements[i].child[j].child[k].products[l];
                            productAction(product);
                        }
                    }
                }
            }
        }

        private void ButtonManageDW_Click(object sender, RoutedEventArgs e)
        {
            DWWindow dwWindow = new DWWindow(this, jsonBase.dvere, jsonBase.okna);
            dwWindow.Owner = this;
            dwWindow.ShowDialog();
        }

        private async void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            #region Manage IDs
            for (int i = 0; i < jsonBase.elements.Count; i++) //izby
            {
                jsonBase.elements[i].id = i;
                for (int j = 0; j < jsonBase.elements[i].child.Count; j++) //Typ
                {
                    jsonBase.elements[i].child[j].id = j;
                    for (int k = 0; k < jsonBase.elements[i].child[j].child.Count; k++) //Typ Typ
                    {
                        InteriorObjectSubtype tt = jsonBase.elements[i].child[j].child[k];
                        tt.id = k;
                        tt.parentid = j;
                        for (int l = 0; l < tt.products.Count; l++)
                        {
                            InteriorProduct product = jsonBase.elements[i].child[j].child[k].products[l];
                            product.id = l;
                            product.room = i;
                            product.typ = j;
                            product.subtyp = tt.category == UIElementCategory.Undefined ? -1 : k;
                        }
                    }
                }
            }
            #endregion

            string serializedJson = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(jsonBase, Formatting.Indented, settings));

            SaveFileDialog sf = new SaveFileDialog();
            sf.AddExtension = true;
            sf.DefaultExt = "*.json";
            sf.Filter = "JSON|*.json";
            sf.FileOk += async (s, ea)  =>
                {
                    Stream stream = sf.OpenFile();
                    StreamWriter sw = new StreamWriter(stream);
                    await sw.WriteAsync(serializedJson);

                    sw.Close();
                };
            sf.ShowDialog();
        }

        private void ButtonAddInterior_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow w = new ProductsWindow(this, jsonBase.elements, jsonBase.manufacturers);
            w.Owner = this;
            w.ShowDialog();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "Save json ?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                ButtonSaveFile_Click(null, null);
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        private void ButtonManageRooms_Click(object sender, RoutedEventArgs e)
        {
            RoomManagementWindow w = new RoomManagementWindow(this, jsonBase);
            w.Owner = this;
            w.ShowDialog();
        }

        private void BTNEditProducts_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow w = new ProductEditWindow(jsonBase.elements);
            w.ShowDialog();
        }

        private void BTNOpenJsonFromURL_Click(object sender, RoutedEventArgs e)
        {
            WindowWithStringResult w = new WindowWithStringResult(async (path) =>
            {
                var result = string.Empty;
                using (var webClient = new System.Net.WebClient())
                {
                    try
                    {
                        result = await webClient.DownloadStringTaskAsync(new Uri(path));
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Nepodarilo sa stiahnut json");
                        return;
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        jsonBase = await Task.Factory.StartNew(() => 
                            {
                                JsonBase b;
                                try
                                {
                                    b = JsonConvert.DeserializeObject<JsonBase>(result, settings);
                                }
                                catch(Exception)
                                {
                                    MessageBox.Show("Neplatny json");
                                    return null;
                                }
                                return b;
                            });

                        TXTSkyboxpath.Text = jsonBase.SkyboxPath;
                        ButtonSaveFile.IsEnabled = true;
                        ButtonManageDW.IsEnabled = true;
                        ButtonAddInterior.IsEnabled = true;
                        ButtonManageManufacturers.IsEnabled = true;
                        ButtonManageRooms.IsEnabled = true;
                        BTNEditProducts.IsEnabled = true;
                    }
                }
            });
            w.ShowDialog();
        }
    }
}
