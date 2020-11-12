using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLocalizationNugetGenerator.Model
{
    [System.SerializableAttribute()]
    public class Data
    {
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "files")]
        public class FilesRoot
        {
            [System.Xml.Serialization.XmlElement("file", IsNullable = true)]
            public List<XmlFile> XmlFiles { get; set; } = new List<XmlFile>();
        }
        public class XmlFile
        {
            [System.Xml.Serialization.XmlAttributeAttribute("src")]
            public string Src { get; set; }
            [System.Xml.Serialization.XmlAttributeAttribute("target")]
            public string Target { get; set; }
        }
    }
}
