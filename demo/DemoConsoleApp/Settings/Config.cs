using Microsoft.Extensions.Configuration;
public class Config
{

    public static IConfigurationRoot GetConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        return config;
    }

    public static string GetConfigFilePath()
    {
        return Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    }

}