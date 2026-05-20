using Microsoft.Extensions.Configuration;
public class Config
{

    public static IConfigurationRoot GetConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        return config;
    }

}