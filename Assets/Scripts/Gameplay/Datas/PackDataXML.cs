using System.Collections.Generic;
using System.Xml.Serialization;

public class PackDataXML {
	[XmlAttribute("packName")] public string packName="undefined";
	[XmlArray("Levels")]
	[XmlArrayItem("Level")]
	public List<LevelDataXML> levelDataXMLs = new List<LevelDataXML>();
}
