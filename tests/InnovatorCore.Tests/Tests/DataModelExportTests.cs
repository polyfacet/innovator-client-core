
using Extensions;
using Innovator.Client.IOM;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class DataModelExportTests
{
    private readonly ArasFixture _fixture;

    public DataModelExportTests(ArasFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Export_a_new_Method_Test()
    {
        // Arrange
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        string testPackageName = "TestPackageExport_" + Guid.NewGuid().ToString().Substring(0, 8);

        // Create a test package
        inn.DataModel().Packages().AddNewPackage(testPackageName);

        // Create a test method item
        Item methodItem = CreateTestMethod(inn);
        Assert.False(methodItem.isError());

        // Add the method to the package
        inn.DataModel().Packages().AddToPackage(methodItem, testPackageName);

        // Export the package
        string exportDir = Path.Combine(Path.GetTempPath(), "aras-exports");
        // Act
        inn.DataModel().Packages().Export(exportDir).Method(methodItem);

        // Assert
        Assert.True(Directory.Exists(exportDir));
        Console.WriteLine($"Exported package to: {exportDir}");
        string subDirExpected =  Path.Combine(exportDir, testPackageName, "Import", "Method");
        Assert.True(Directory.Exists(subDirExpected), $"Expected export subdirectory does not exist: {subDirExpected}");
        string expectedFileName = methodItem.getProperty("name");
        string expectedFilePath = Path.Combine(subDirExpected, expectedFileName + ".xml");
        Assert.True(File.Exists(expectedFilePath), $"Expected export file does not exist: {expectedFilePath}");

        // Clean up - delete the created package and method
        methodItem.apply("delete");
        Item packageDefinition = inn.GetItemByName("PackageDefinition", testPackageName);
        packageDefinition.apply("delete");
        // Directory.Delete(exportDir, recursive: true);
    }

    [Fact]
    public void Export_Method_HC_TestMethod_and_validate_with_Export_Resource_file()
    {
        // Arrange
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();

        // Locate the resource XML by walking up parent directories (robust for test output folders)
        
        string resourcePath = GetTestMethodResourcePath("HC_Test_CsharpMethod.xml");
        Assert.True(File.Exists(resourcePath), $"Resource file not found: {resourcePath}");

        string aml = File.ReadAllText(resourcePath);

        // Apply the AML to create the Method
        Item methodItem = inn.applyAML(aml);
        Assert.False(methodItem.isError(), $"Applying AML failed with error: {methodItem.getErrorString()}");

       
        // Act
        // Export the method
        string exportDir = Path.Combine(Path.GetTempPath(), "aras-exports");
        if (Directory.Exists(exportDir)) Directory.Delete(exportDir, recursive: true);
        inn.DataModel().Packages().Export(exportDir).Method(methodItem);

        // Assert
        // Find the exported file by name
        string expectedFileName = methodItem.getProperty("name");
        var exportedFiles = Directory.Exists(exportDir)
            ? Directory.GetFiles(exportDir, expectedFileName + ".xml", SearchOption.AllDirectories)
            : Array.Empty<string>();

        Assert.True(exportedFiles.Length > 0, $"Exported file not found for {expectedFileName} in {exportDir}");
        string exportedFilePath = exportedFiles.First();

        // Compare normalized XML
        string expectedXml = File.ReadAllText(resourcePath);
        string actualXml = File.ReadAllText(exportedFilePath);

        var expectedItem = NormalizeItemXml(expectedXml);
        var actualItem = NormalizeItemXml(actualXml);

        string NormalizeString(string s) => Regex.Replace(s, "\\s+", " ").Trim();
        Assert.Equal(NormalizeString(expectedItem.ToString(SaveOptions.DisableFormatting)), NormalizeString(actualItem.ToString(SaveOptions.DisableFormatting)));

        // Cleanup
        try { methodItem.apply("delete"); } catch { }
        try { if (Directory.Exists(exportDir)) Directory.Delete(exportDir, recursive: true); } catch { }
    }

    private XElement NormalizeItemXml(string xml)
    {
        var doc = XDocument.Parse(xml);
        var item = doc.Descendants().FirstOrDefault(e => string.Equals(e.Name.LocalName, "Item", StringComparison.OrdinalIgnoreCase)) ?? doc.Root;
        if (item == null) throw new Exception("No <Item> element found in XML.");
        var clone = new XElement(item);
        return clone;
    }

    private string GetTestMethodResourcePath(string fileName)
    {
        string resourceRelative = Path.Combine("Resources", "2025", fileName);       
        string dir = Path.GetFullPath(AppContext.BaseDirectory);
        string resourcePath = Path.Combine(dir, resourceRelative);
        return resourcePath;
    }

    private static Item CreateTestMethod(Innovator.Client.IOM.Innovator inn)
    {
        Item methodItem = inn.newItem("Method", "add");
        methodItem.setProperty("name", "TestMethod_" + Guid.NewGuid().ToString().Substring(0, 8));
        methodItem.setProperty("method_type", "Server");
        methodItem.setProperty("method_code", "return 'test';");
        methodItem = methodItem.apply();
        return methodItem;
    }
}