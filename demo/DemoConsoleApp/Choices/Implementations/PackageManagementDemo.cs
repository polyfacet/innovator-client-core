
namespace DemoConsoleApp.Choices;
public class PackageManagementDemo : ChoiceBase
{
    public override string Name => "Package Management Demo";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        
        LogLine("Retrieving package information for 'Part' item type...");
        Item partItemType = inn.GetItemByName("ItemType", "Part");
        PackageInfo partItemTypePackageInfo = inn.DataModel().Packages().GetInfo(partItemType);
        LogPackageInfo(partItemTypePackageInfo);

        // Create new method
        // but first check if a method with the same name already exists to avoid conflicts in repeated runs of the demo
        string methodName = "Demo Method";
        Item existingMethod = inn.GetItemByName("Method", methodName);
        if (!existingMethod.isError())
        {
            LogLine("A method with the name '" + methodName + "' already exists. Deleting it...");
            existingMethod.apply("delete");
            LogLine("Existing method deleted.");
        }
        LogLine("Creating a new method item...");
        Item methodItem = inn.newItem("Method", "add");
        methodItem.setProperty("name", methodName);
        methodItem.setProperty("method_type", "Server");
        methodItem.setProperty("method_code", "return 'Hello from the demo method!';");
        methodItem = methodItem.apply();
        LogLine("New method item created successfully. Name: [green]" + methodItem.getProperty("name") + "[/]");

        LogLine("Retrieving package information for the new method...");
        PackageInfo methodPackageInfo = inn.DataModel().Packages().GetInfo(methodItem);
        LogPackageInfo(methodPackageInfo);
        
        // Add method to package and show info
        LogLine("Adding method to package...");
        string targetPackageName = "Demo Package";
        Item targetPackage = inn.DataModel().Packages().AddNewPackage(targetPackageName, skipIfExists: true);
        Item packageElementItem = inn.DataModel().Packages().AddToPackage(methodItem, targetPackageName);
        LogLine("Method added to package. Retrieving package information again to confirm...");
        PackageInfo updatedMethodPackageInfo = inn.DataModel().Packages().GetInfo(methodItem);
        LogPackageInfo(updatedMethodPackageInfo);

        // Cleanup
        if (AnsiConsole.Confirm("Demo complete. Do you want to clean up the created package and method items?"))
        {
            LogLine("Cleaning up: removing method from package and deleting created items...");
            packageElementItem.apply("delete");
            targetPackage.apply("delete");
            methodItem.apply("delete");
        }
        else
        {
            LogLine("Cleanup skipped. Remember to manually delete the created method and package items to avoid clutter.");
        }
    }

    private void LogPackageInfo(PackageInfo packageInfo)
    {
        LogLine($"\t Package Package(Definition) Name: [green]{packageInfo.PackageName}[/]", includeTimestamp: false);
        LogLine($"\t Package Package Group: [green]{packageInfo.PackageGroup}[/]", includeTimestamp: false);
        LogLine($"\t Package Element Name: [green]{packageInfo.PackageElementName}[/]", includeTimestamp: false);
        LogLine($"\t Package Element Created on: [green]{packageInfo.PackageElementCreatedOn}[/]", includeTimestamp: false);
    }
}