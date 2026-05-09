
using Innovator.Client.IOM;

public class ArasFixture : IAsyncLifetime
{

    private Innovator.Client.IOM.Innovator? _adminInn;
    public Innovator.Client.IOM.Innovator GetAdminInn()
    {
        if (_adminInn == null)
        {
            _adminInn = DefaultEnvConfig.GetAdminSession();
        }
        return _adminInn;
    }
    public async ValueTask InitializeAsync()
    {
        // Setup: Körs EN gång innan det första testet i hela projektet startar
        await Task.Delay(1); 
    }

    public async ValueTask DisposeAsync()
    {
        // Teardown: Körs EN gång efter att ALLA tester i projektet är klara
        Cleanup();
    }

    private void Cleanup()
    {
        DeleteTestUsers();
    }

    private void DeleteTestUsers()
    {
        Innovator.Client.IOM.Innovator inn = GetAdminInn();
        Item users = inn.newItem("User", "get");
        users.setProperty("last_name", "Doe");
        users = users.apply();
        if (!users.isError())
        {
            for (int i = 0; i < users.getItemCount(); i++)
            {
                Item user = users.getItemByIndex(i);
                user.apply("delete");
            }
        }
    }
}