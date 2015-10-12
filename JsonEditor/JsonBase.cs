using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class JsonBase
    {
        public string SkyboxPath { get; set; }
        public string BaseAssetbundlePath { get; set; }
        public Balkony balkony { get; set; }
        public DW okna { get; set; }
        public DW dvere { get; set; }
        public List<ManufacturerItem> manufacturers { get; set; }
        public List<RoomItem> elements { get; set; }

        public JsonBase()
        {
            balkony = new Balkony();
            okna = new DW();
            dvere = new DW();
            manufacturers = new List<ManufacturerItem>();
            elements = new List<RoomItem>();
        }
    }

    public enum UIElementCategory
    {
        Undefined,
        Manufacturer,
        Room,
        ProductType,
        ProductTypeType,
        Product
    }
}
