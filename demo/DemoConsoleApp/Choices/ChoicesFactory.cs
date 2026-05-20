namespace DemoConsoleApp.Choices;
public class ChoicesFactory
{
    public static List<IChoice> GetChoices()
    {
        return new List<IChoice>
        {            
            new EcoItemTypeInfo(),
            new CreateDeleteUser(),
            new ReleasePartViaECO(),
            // Add more choices here

            new Exit()
        };
    }

    public static Dictionary<string, IChoice> GetChoicesDictionary()
    {
        return GetChoices().ToDictionary(c => c.Name, c => c);
    }
}