
using System.Text;
using System.Xml;
using System.Xml.Linq;

public class Export
{
    private Innovator.Client.IOM.Innovator Inn { get; }
    private string ExportDir { get; }

    public Export(Innovator.Client.IOM.Innovator inn, string exportDir)
    {
        Inn = inn;
        ExportDir = exportDir;
    }

    public void Method(Item methodItem)
    {
        string filePath = GetExportFilePath(methodItem);
        string xmlContent = CreateMethodExportXml(methodItem);
        SaveToFile(filePath, xmlContent);
    }


    private string GetExportFilePath(Item item)
    {
        PackageInfo packageInfo = Inn.DataModel().Packages().GetInfo(item);
        string packageName = packageInfo.PackageName;
        if (string.IsNullOrEmpty(packageName)) packageName = "UnknownPackage";
        
        Item firstGeneration = Inn.GetFirstGeneration(item);
        string name = firstGeneration.getProperty("name", firstGeneration.getID());
        string filePath = Path.Combine(ExportDir, packageName, "Import", item.getType(),  name+".xml");
        return filePath;
    }

    private string CreateMethodExportXml(Item methodItem)
    {
        Item strippedMethod = Inn.newItem(methodItem.getType(), "get");
        strippedMethod.setID(methodItem.getID());
        strippedMethod.setAttribute("select", "method_code,method_type,execution_allowed_to,name");
        strippedMethod = strippedMethod.apply();
        
        XDocument doc = XDocument.Parse(strippedMethod.dom.InnerXml);

        XElement sourceItem = doc.Descendants("Item").First();
        string rawCode = sourceItem.Element("method_code")?.Value ?? "";

        // Create a new XML structure with the desired formatting and CDATA-wrapping for method_code
        XElement amlItem = new XElement("Item",
            new XAttribute("type", sourceItem.Attribute("type")?.Value ?? ""),
            new XAttribute("id", sourceItem.Attribute("id")?.Value ?? ""),
            new XAttribute("action", "add"),
            sourceItem.Element("execution_allowed_to"),
            new XElement("method_code", new XCData(rawCode)), // <-- Tvingar CDATA-wrapping [1]
            sourceItem.Element("method_type"),
            sourceItem.Element("name")
        );
          
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = " ", 
            OmitXmlDeclaration = true // Omit the XML declaration
        };

        // Wrap the Item inside an <AML> root so exported files include the AML wrapper
        XElement amlRoot = new XElement("AML", amlItem);

        // Write the XML content to a string
        using (var stringWriter = new StringWriter())
        using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
        {
            amlRoot.WriteTo(xmlWriter);
            xmlWriter.Flush();

            string formattedXml = stringWriter.ToString();
            return formattedXml;
        }
    }   

    private void SaveToFile(string filePath, string xmlContent)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (directory == null) throw new InvalidOperationException("Invalid file path: " + filePath);
        if (!Directory.Exists(directory))        {
            Directory.CreateDirectory(directory);
        }
        using (var writer = new StreamWriter(filePath))
        {
            writer.Write(xmlContent);
        }
    }
}
