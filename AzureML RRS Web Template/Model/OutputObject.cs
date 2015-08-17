using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureMLInterface.Model
{
    public class OutputObject
    {
        string name ="";
        List<string> values = new List<string>();

        public List<string> Values
        {
            get { return values; }
            set { values = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}