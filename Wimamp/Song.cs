using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Wimamp
{
    [XmlRoot(Namespace = "Wimamp")]
    [XmlInclude(typeof(Song))]
    [Serializable]
    public class Song
    {
        private string _name;
        public string Uri { get; set; }
        public string Duration { get; set; }

        public string Name
        {
            get { return _name; }
            set { _name = value;}
        }

    }
}