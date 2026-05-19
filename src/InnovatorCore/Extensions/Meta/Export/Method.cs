using System.Xml;

namespace Meta.Export;

public class Method : IItemExport
{
    private Innovator.Client.IOM.Innovator Inn;
    private string _methodName;

    private Item? _methodItem;
    public Method(Innovator.Client.IOM.Innovator inn, string methodName)
    {
        Inn = inn;
        _methodName = methodName;
        _methodItem = inn.GetItemByName("Method", _methodName);
    }

    public Item Item => _methodItem ?? throw new InvalidOperationException("Method item not found");

    public XmlDocument CreateXml()
    {
        throw new NotImplementedException();
    }
}