
using Extensions;
using Innovator.Client.IOM;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class DataModelTests
{
    private readonly ArasFixture _fixture;

    public DataModelTests(ArasFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DataModel_Part_ECO_Demo_Test()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        
        ItemType ecoItemType = inn.DataModel().ItemType("Express ECO");
        Assert.True(ecoItemType.IsWorkflowEnabled());
        Assert.NotEmpty(ecoItemType.WorkflowMaps());
        // foreach (var workflowMap in ecoItemType.WorkflowMaps())
        // {
        //     workflowMap.WorkflowMapItem.getProperty("name");
        //     Console.WriteLine("Workflow Map: " + workflowMap.WorkflowMapItem.getProperty("name"));
        //     foreach (var activityTemplate in workflowMap.ActivityTemplates)
        //     {
        //         Console.WriteLine("  Activity Template: " + activityTemplate.Name);
        //         foreach (var path in activityTemplate.WorkflowMapPaths)
        //         {
        //             Console.WriteLine("    Workflow Map Path: " + path.Name + ", to Activity Template: " + path.ToActivity.Name);
        //         }
        //     }
        // }

        ItemType partItemType =  inn.DataModel().ItemType("Part");
        Assert.False(partItemType.IsWorkflowEnabled());
        Assert.True(partItemType.Item.LastModified() > DateTime.MinValue);
        Console.WriteLine("Part ItemType last modified: " + partItemType.Item.LastModified());

        bool exists = partItemType.Property("item_number").Exists();
        Assert.True(exists);
        
        ArasProperty prop = partItemType.Property("item_number"); 
        
        string propertyType = prop.DataType();
        Assert.Equal("String", propertyType, ignoreCase: true);
        Assert.True(prop.IsRequired());
        Assert.False(prop.HiddenOnMainSearch());
        Assert.False(prop.HiddenOnRelationships());
        Assert.True(prop.SortOrder() > 0);

        partItemType.Properties().ForEach(p => {
            Console.WriteLine($"Property: {p.PropertyRelation?.getProperty("name")}, type = {p.DataType()}"); //, required = {p.IsRequired()}");
        });

    }

    [Fact]
    public void DataModel_Packages_Demo_Test()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        
        Item partItemType = inn.GetItemByName("ItemType", "Part");
        PackageInfo partPackageInfo = inn.DataModel().Packages().GetInfo(partItemType);
                
        string expectedPackageElementName = "Part";
        Assert.Equal(expectedPackageElementName, partPackageInfo.PackageElementName);
        DateTime expectedCreatedOnAfter = new DateTime(2024, 1, 1);
        Assert.True(partPackageInfo.PackageElementCreatedOn > expectedCreatedOnAfter);
        string expectedPackageGroup = "ItemType";
        Assert.Equal(expectedPackageGroup, partPackageInfo.PackageGroup);
        string expectedPackageName = "com.aras.innovator.solution.PLM";
        Assert.Equal(expectedPackageName, partPackageInfo.PackageName);
    }

    [Fact]
    public void AddNewPackage_creates_package()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        string testPackageName = "TestPackage_" + Guid.NewGuid().ToString().Substring(0, 8);

        inn.DataModel().Packages().AddNewPackage(testPackageName);

        // Verify that the package was created
        Item packageDefinition = inn.GetItemByName("PackageDefinition", testPackageName);
        Assert.NotNull(packageDefinition);
        Assert.False(packageDefinition.isError());
        Assert.Equal(testPackageName, packageDefinition.getProperty("name"));

        // Clean up - delete the created package
        packageDefinition.apply("delete");

    }

    [Fact]
    public void AddNewPackage_skips_if_exists()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        string testPackageName = "TestPackageSkip_" + Guid.NewGuid().ToString().Substring(0, 8);

        // Create the package first
        inn.DataModel().Packages().AddNewPackage(testPackageName);
        Item firstPackage = inn.GetItemByName("PackageDefinition", testPackageName);
        string firstId = firstPackage.getID();

        // Try to create again with skipIfExists = true
        inn.DataModel().Packages().AddNewPackage(testPackageName, skipIfExists: true);

        // Verify the same package exists (ID should be unchanged)
        Item secondPackage = inn.GetItemByName("PackageDefinition", testPackageName);
        Assert.Equal(firstId, secondPackage.getID());

        // Clean up - delete the created package
        firstPackage.apply("delete");
        secondPackage.apply("delete");
    }

    [Fact]
    public void AddToPackage_Adds_Item_to_package()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        string testPackageName = "TestPackageAdd_" + Guid.NewGuid().ToString().Substring(0, 8);

        // Create a test package
        inn.DataModel().Packages().AddNewPackage(testPackageName);

        // Create a test method item
        Item methodItem = CreateTestMethod(inn);

        Assert.False(methodItem.isError());

        // Add the method to the package
        inn.DataModel().Packages().AddToPackage(methodItem, testPackageName);

        // Verify the method is now in the package
        PackageInfo packageInfo = inn.DataModel().Packages().GetInfo(methodItem);
        Assert.Equal(testPackageName, packageInfo.PackageName);
        Assert.Equal("Method", packageInfo.PackageGroup);

        // Clean up - delete the created package and method
        methodItem.apply("delete");
        Item packageDefinition = inn.GetItemByName("PackageDefinition", testPackageName);
        packageDefinition.apply("delete");
    }
    

    [Fact]
    public void ListAll_Packages_Test()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        var packages = inn.DataModel().Packages().ListAll();
        Assert.NotNull(packages);
        Assert.True(packages.Count > 0);
        Console.WriteLine("Packages:");
        // packages.ForEach(p => Console.WriteLine($"- {p.getProperty("name")} (ID: {p.getID()})"));
        packages.ForEach(p => Console.WriteLine($"- {p.getProperty("name")}"));
    }

    [Fact]
    public void Find_core_package_definition_Test()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        string corePackageName = "com.aras.innovator.core";
        Item packageDefinition = inn.DataModel().Packages().FindPackageDefinition(corePackageName);
        Assert.NotNull(packageDefinition);
        Assert.False(packageDefinition.isError());
        Assert.Equal(corePackageName, packageDefinition.getProperty("name"));
        Console.WriteLine($"Found core package definition: {packageDefinition.getProperty("name")} (ID: {packageDefinition.getID()}) (Created on: {packageDefinition.getProperty("created_on")})");
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