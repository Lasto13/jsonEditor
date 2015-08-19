using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class InteriorObjectType
    {
        public UIElementCategory category { get; private set; }
        public string uidisplayname { get; set; }
        public int id { get; set; }
        public List<InteriorObjectSubtype> child { get; set; }

        public InteriorObjectType()
        {
            category = UIElementCategory.ProductType;
            child = new List<InteriorObjectSubtype>();
        }
    }
}
