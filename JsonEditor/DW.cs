using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class DW
    {
        public int pocet { get; set; }
        public List<DWItem> child { get; set; }

        public DW()
        {
            child = new List<DWItem>();
        }
    }
}
