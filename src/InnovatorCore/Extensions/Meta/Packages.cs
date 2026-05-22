using Extensions;

public class Packages
{
    private Innovator.Client.IOM.Innovator Inn;

    public Packages(Innovator.Client.IOM.Innovator inn)
    {
        Inn = inn;
    }

    public PackageInfo GetInfo(Item item)
    {
        var info = new PackageInfo();
        if (item == null || item.isError()) return info;

        Item? packageElement = GetPackageElement(item);
        if (packageElement == null) return info;
        PopulatePackageElementInfo(info, packageElement);

        Item? packageGroup = GetPackageGroup(packageElement);
        if (packageGroup == null) return info;
        info.PackageGroup = packageGroup.getProperty("name", "");

        Item? packageDefinition = GetPackageDefinition(packageGroup);
        if (packageDefinition == null) return info;
        info.PackageName = packageDefinition.getProperty("name", "");

        return info;
    }

    private Item? GetPackageElement(Item item)
    {
        Item packageElementRequest = Inn.newItem("PackageElement", "get");
        packageElementRequest.setProperty("element_id", item.getProperty("config_id"));
        var packageElementResponse = packageElementRequest.apply();

        if (packageElementResponse.isError()) return null;
        if (packageElementResponse.getItemCount() == 0) return null;

        return packageElementResponse.getItemByIndex(0);
    }

    private void PopulatePackageElementInfo(PackageInfo info, Item packageElement)
    {
        info.PackageElementName = packageElement.getProperty("name", info.PackageElementName);

        if (DateTime.TryParse(packageElement.getProperty("created_on", ""), out var createdOn))
        {
            info.PackageElementCreatedOn = createdOn;
        }
    }

    private Item? GetPackageGroup(Item packageElement)
    {
        string packageId = packageElement.getProperty("source_id", "");
        if (String.IsNullOrEmpty(packageId)) return null;

        var packageGroup = Inn.GetItem("PackageGroup", packageId);
        return packageGroup.isError() ? null : packageGroup;
    }

    private Item? GetPackageDefinition(Item packageGroup)
    {
        string packageDefId = packageGroup.getProperty("source_id", "");
        if (String.IsNullOrEmpty(packageDefId)) return null;

        var packageDefinition = Inn.GetItem("PackageDefinition", packageDefId);
        return packageDefinition.isError() ? null : packageDefinition;
    }

    public Item AddNewPackage(string packageName, bool skipIfExists = false)
    {
        if (String.IsNullOrEmpty(packageName)) return Inn.newError("Package name cannot be empty");

        if (skipIfExists)
        {
            Item existingPackage = Inn.GetItemByName("PackageDefinition", packageName);
            if (!existingPackage.isError()) return existingPackage;
        }

        Item packageDefinition = Inn.newItem("PackageDefinition", "add");
        packageDefinition.setProperty("name", packageName);
        packageDefinition = packageDefinition.apply();

        if (packageDefinition.isError()) return packageDefinition;

        // Item packageGroup = Inn.newItem("PackageGroup", "add");
        // packageGroup.setProperty("name", packageName);
        // packageGroup.setProperty("source_id", packageDefinition.getID());
        // packageGroup = packageGroup.apply();
        return packageDefinition;
    }

    public Item AddToPackage(Item item, string packageName)
    {
        if (item == null || item.isError() || String.IsNullOrEmpty(packageName))
            return Inn.newError("Invalid item or package name");

        Item packageDefinition = Inn.GetItemByName("PackageDefinition", packageName);
        if (packageDefinition.isError()) return packageDefinition;

        string packageGroupName = item.getType();
        Item packageGroup = Inn.newItem("PackageGroup", "get");
        packageGroup.setProperty("name", packageName);
        packageGroup.setProperty("source_id", packageDefinition.getID());
        packageGroup = packageGroup.apply();

        if (packageGroup.isError()) {
            // Try to create the package group if it doesn't exist
            packageGroup = Inn.newItem("PackageGroup", "add");
            packageGroup.setProperty("name", packageGroupName);
            packageGroup.setProperty("source_id", packageDefinition.getID());
            packageGroup = packageGroup.apply();
        } 
          

        string packageGroupId = packageGroup.getID();

        string name = item.getProperty("keyed_name");

        Item packageElement = Inn.newItem("PackageElement", "add");
        packageElement.setProperty("element_id", item.getProperty("config_id"));
        packageElement.setProperty("element_type", item.getType());
        packageElement.setProperty("name", name);
        packageElement.setProperty("source_id", packageGroupId);
        packageElement = packageElement.apply();
        return packageElement;
    }

    public List<Item>? ListAll()
    {
        return Inn.newItem("PackageDefinition", "get").apply().ToList();
    }

    public Item FindPackageDefinition(string packageName)
    {
        if (String.IsNullOrEmpty(packageName)) return Inn.newError("Package name cannot be empty");

        Item packageDefinition = Inn.GetItemByName("PackageDefinition", packageName);
        return packageDefinition;
    }
}
