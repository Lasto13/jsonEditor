using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    public class Balkony
    {
        public int Pocet { get; set; }
        public List<BalkonItem> child { get; set; }

        public Balkony()
        {
            child = new List<BalkonItem>();
        }
    }
}
