using System;
using System.Collections.Generic;
using System.Text;

namespace WpfBu.Models
{
    public class RootForm
    {
        public string id { get; set; }
        public string text { get; set; }
        public object userMenu { get; set; }
        public object userContent { get; set; }
        public virtual void start(object o)
        { 
        }
        public override string ToString()
        {
            return text;
        }
    }
}
