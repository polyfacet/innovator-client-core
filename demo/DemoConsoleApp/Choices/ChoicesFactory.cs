

namespace DemoConsoleApp.Choices;
public class ChoicesFactory
{
    public static List<IChoice> GetChoices()
    {
        return new List<IChoice>
        {            
            new ArasConnectionsChoice(),
            new ItemTypeInfo(),
            new CreateDeleteUser(),
            new ReleasePartViaECO(),
            new GetMembershipTree(),
            new PackageManagementDemo(),
            new ExportMethod(),
            new MethodWhereUsed(),
            // new Search(),
            // new ListSavedItems(),
            new SearchListDelete(),
            // Add more choices here

            new Exit()
        };
    }

    public static Dictionary<string, IChoice> GetChoicesDictionary()
    {
        return GetChoices().ToDictionary(c => c.Name, c => c);
    }
}