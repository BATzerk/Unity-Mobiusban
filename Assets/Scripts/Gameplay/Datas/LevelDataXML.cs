using System.Xml;
using System.Xml.Serialization;

public class LevelDataXML {
	[XmlAttribute("desc")] public string desc;
    //[XmlAttribute("diff")] public int difficulty;
	[XmlAttribute("fueID")] public string fueID;
	[XmlAttribute("numColors")] public int numColors=3;

	[XmlAttribute("layout")] public string layout;

}