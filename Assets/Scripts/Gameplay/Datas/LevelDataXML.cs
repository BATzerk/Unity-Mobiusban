using System.Xml;
using System.Xml.Serialization;

public class LevelDataXML {
	[XmlAttribute("desc")] public string desc;
	[XmlAttribute("fueID")] public string fueID;
    
    [XmlAttribute("wrapH")] public int wrapH;
    [XmlAttribute("wrapV")] public int wrapV;
	[XmlAttribute("layout")] public string layout;
    
    [XmlAttribute("doShowEchoes")] public bool doShowEchoes=true;
    [XmlAttribute("zoom")] public float zoom=0.5f;
}