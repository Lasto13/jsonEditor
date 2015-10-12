using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class InteriorProduct
    {
        public UIElementCategory category { get; private set; }
        public string uidisplayname { get; set; }
        public string path { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string descriptionfordetail { get; set; }
        public string pathformanufacturer { get; set; }
        public int id { get; set; }
        public int room { get; set; }
        public int typ { get; set; }
        public int subtyp { get; set; }
        public int manufacturerid { get; set; }
        public string manufacturername { get; set; }
        public string hash { get; set; }
        public uint crc { get; set; }
        public ObjectPlacementOptions placementOptions { get; set; }

        public InteriorProduct()
        {
            category = UIElementCategory.Product;
        }
    }

    [System.Flags]
    public enum ObjectPlacementOptions
    {
        None = 0,
        Wall = 1,
        Floor = 2,
        Object = 4,
        Ceiling = 8
    }
}
