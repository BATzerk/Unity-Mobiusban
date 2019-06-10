using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("PackCollection")]
public class PackCollectionDataXML {
	[XmlArray("Packs")]
	[XmlArrayItem("LevelPack")]
	public List<PackDataXML> packDataXMLs = new List<PackDataXML>();
}
