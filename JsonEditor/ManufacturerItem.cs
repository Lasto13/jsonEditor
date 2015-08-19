using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class ManufacturerItem
    {
        public UIElementCategory category { get; private set; }
        public string uidisplayname { get; set; }
        public int id { get; set; }

        public ManufacturerItem()
        {
            category = UIElementCategory.Manufacturer;
        }
    }
}
