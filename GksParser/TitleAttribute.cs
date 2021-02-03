using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GksParser
{
    class TitleAttribute : System.Attribute
    {
        public string Title { get; set; }
        public TitleAttribute(string title)
        {
            this.Title = title;
        }
    }
}
