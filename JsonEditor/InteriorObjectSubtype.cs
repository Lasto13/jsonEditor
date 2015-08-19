using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class InteriorObjectSubtype
    {
        public UIElementCategory category { get; set; }
        public string uidisplayname { get; set; }
        public int id { get; set; }
        public int parentid { get; set; }
        public List<InteriorProduct> products { get; set; }

        public InteriorObjectSubtype()
        {
            products = new List<InteriorProduct>();
        }
        public InteriorObjectSubtype(UIElementCategory cat)
        {
            products = new List<InteriorProduct>();
            category = cat;
        }

        public bool ShouldSerializeparentid()
        {
            return category != UIElementCategory.Undefined;
        }
    }
}
