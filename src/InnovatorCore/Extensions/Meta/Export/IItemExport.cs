using System.Xml;

namespace Meta.Export;
public interface IItemExport
{
    Item Item { get; }
    XmlDocument CreateXml();
}