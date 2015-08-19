using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class RoomItem
    {
        public UIElementCategory category { get; private set; }
        public string uidisplayname { get; set; }
        public int id { get; set; }
        public List<InteriorObjectType> child { get; set; }

        public RoomItem()
        {
            child = new List<InteriorObjectType>();
            category = UIElementCategory.Room;
        }
    }
}
