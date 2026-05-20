


namespace DemoConsoleApp.Choices;

public class ReleasePartViaECO : IChoice
{
    public string Name => "Release Part via ECO";

    public void Execute(Innovator.Client.IOM.Innovator inn)
    {
        // Create a new Part
        Item partItem = inn.newItem("Part", "add");
        partItem.setProperty("item_number", "Test Part " + Generators.ShortGUID());
        partItem.setProperty("name", "Test Part");
        partItem = partItem.apply();
        if (ErrorHandler.HandleError(partItem, "creating part")) return;
        ConsoleLog.Line($"Created Part with item number: [green]{partItem.getProperty("item_number")}[/]");

        // Create a new ECO
        Item ecoItem = inn.newItem("Express ECO", "add");
        ecoItem.setProperty("title", "Test ECO " + Generators.ShortGUID());
        ecoItem.setProperty("item_number", "Test ECO " + Generators.ShortGUID());
        ecoItem = ecoItem.apply();
        if (ErrorHandler.HandleError(ecoItem, "creating ECO")) return;
        ConsoleLog.Line($"Created ECO with item number: [green]{ecoItem.getProperty("item_number")}[/]");

        // Add the Part to the ECO
        ECO eco = new ECO(ecoItem);
        Item affectedItem = eco.AddAffectedItem(partItem, "Release");
        if (ErrorHandler.HandleError(affectedItem, "adding affected item to ECO")) return;
        ConsoleLog.Line($"Added Part [blue]{partItem.getProperty("item_number")}[/] to ECO [blue]{ecoItem.getProperty("item_number")}[/] with change type 'Release'.");

        // Release the ECO via signoffs
        ConsoleLog.Line($"Releasing ECO [blue]{ecoItem.getProperty("item_number")}[/] via signoffs...");
        ConsoleLog.Line("Signing off first ...");
        Item signOffResult = eco.SignOff("");
        if (ErrorHandler.HandleError(signOffResult, "initial sign off")) return;
        Item claimResult = eco.ClaimSignOffs("Start Work", inn);
        if (ErrorHandler.HandleError(claimResult, "claiming sign off")) return;
        ConsoleLog.Line("Signing off 'Start Work'...");
        signOffResult = eco.SignOff("Start Work");
        if (ErrorHandler.HandleError(signOffResult, "signing off")) return;
        claimResult = eco.ClaimSignOffs("Close Change", inn);
        if (ErrorHandler.HandleError(claimResult, "claiming sign off")) return;
        ConsoleLog.Line("Signing off 'Close Change'...");
        signOffResult = eco.SignOff("Close Change");
        if (ErrorHandler.HandleError(signOffResult, "signing off")) return;

        string configId = partItem.getProperty("config_id", "N/A");
        Item releasedPart = inn.GetItemByConfigId("Part", configId);
       
        ConsoleLog.Line($"Part [blue]{partItem.getProperty("item_number")}[/] is now in state: [green]{releasedPart.getProperty("state", "N/A")}[/].");        
    }

    
}