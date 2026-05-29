
using Extensions;
using Innovator.Client.Model;
using Workflows;

namespace DemoConsoleApp.Choices;

public class ExportMethod : ChoiceBase
{
    public override string Name => "Export Method";
    private int maxRecords = 5;

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {

        // TODO: Prompt for output dir, if not exist prompt if create, save this as a preferred export dir

        Item methodItems = inn.newItem("Method", "get");
        methodItems.setAttribute("maxRecords", maxRecords.ToString());
        methodItems.setAttribute("orderBy", "modified_on DESC");
        methodItems = methodItems.apply();

        Dictionary<string, Item> methods = new Dictionary<string, Item>();
        foreach (Item method in methodItems.ToList())
        {
            string key = method.getProperty("name", method.getID()) + "\t Modified on:" + method.getProperty("modified_on");
            methods[key] = method;
        }

        if (methods.Count == 0)
        {
            Console.WriteLine("No Method items found to export.");
            return;
        }

        var selectionPrompt =new SelectionPrompt<string>()
            .Title("\n Select a Method to export:")
            // .PageSize(20)
            .AddChoices(methods.Keys);
        selectionPrompt.AddChoice("Exit");

        var val = AnsiConsole.Prompt(selectionPrompt);
       AnsiConsole.MarkupLine($"You selected: [green]{val}[/]");
       if (val == "Exit") return;
       string methodName = val;
       Item methodItem = methods[methodName];
        
       string exportDir = Path.Combine(Environment.CurrentDirectory, "Exports");
       var exporter = new Export(inn, exportDir);
       exporter.Method(methodItem);
       AnsiConsole.MarkupLine($"[green]Method exported to {exportDir}[/]");
               
    }
}