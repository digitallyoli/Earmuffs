using System.Xml;

namespace Earmuffs.Helpers;

public class SettingsHelper
{
    private static readonly string _localDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"oli.digital/Earmuffs/");

    public static T Get<T>(string id, T defaultValue)
    {
        XmlNode? node = GetConfig().SelectSingleNode($@"/Settings/{id}");
        return node == null ? defaultValue : (T)Convert.ChangeType(node.InnerText, typeof(T));
    }

    public static void Set(string id, object value)
    {
        XmlDocument doc = GetConfig();
        XmlNode? node = doc.SelectSingleNode($@"/Settings/{id}");
        if (node != null)
        {
            node.InnerText = value.ToString() ?? "";
        }
        else
        {
            XmlNode newNode = doc.CreateNode(XmlNodeType.Element, id, doc.NamespaceURI);
            newNode.InnerText = value.ToString() ?? "";
            doc.SelectSingleNode(@"/Settings")?.AppendChild(newNode);
        }
        doc.Save(new Uri(doc.BaseURI).LocalPath);
    }

    private static readonly string _file = Path.Combine(_localDataPath, "Earmuffs.config");

    private static XmlDocument GetConfig()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_file) ?? "");
        if (!File.Exists(_file))
        {
            CreateNewConfig();
        }
        XmlDocument doc = new();
        try
        {
            doc.Load(_file);
        }
        catch (XmlException)
        {
            CreateNewConfig();
            doc.Load(_file);
        }
        return doc;
    }

    private static void CreateNewConfig()
    {
        using XmlWriter writer = XmlWriter.Create(_file);
        writer.WriteStartDocument();
        writer.WriteStartElement("Settings");
        writer.WriteEndElement();
        writer.WriteEndDocument();
        writer.Flush();
    }
}