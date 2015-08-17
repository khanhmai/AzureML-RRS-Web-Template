using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParameterIO
{
    public class AMLParam
    {
        [XmlElement("Name")]
        string name = "";

        [XmlElement("Type")]
        string type = "";

        [XmlElement("Format")]
        string format = "";

        [XmlElement("Enum")]
        List<string> strEnum = new List<string>();

        [XmlElement("MinValue")]
        string minValue = "0";

        [XmlElement("MaxValue")]
        string maxValue = "100";


        [XmlElement("DefaultValue")]
        string defaultValue = "";

        [XmlElement("Description")]
        string description = "";

        [XmlElement("Alias")]
        string alias = "";

        [XmlElement("Enable")]
        bool enable = true;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public List<string> StrEnum
        {
            get { return strEnum; }
            set { strEnum = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public string MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        public string MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }


    }
}