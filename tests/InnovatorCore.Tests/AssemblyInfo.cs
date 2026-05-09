

// Register global setup/teardown
[assembly: AssemblyFixture(typeof(ArasFixture))]

// To have Console.WriteLine visible in test-output in v3 together with:
// dotnet test --logger "console;verbosity=detailed"
[assembly: CaptureConsole]
