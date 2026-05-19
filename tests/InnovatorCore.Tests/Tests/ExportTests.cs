using System.Xml;
using Meta.Export;

public class ExportTests
{
    private readonly ArasFixture _fixture;

    public ExportTests(ArasFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Export_Method_Test()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        Method methodExport = new Method(inn, "PE_clean_has_files_prop");
        XmlDocument methodXml = methodExport.CreateXml();

        Assert.NotNull(methodXml); // Placeholder assertion, replace with actual checks on the XML content
        // TODO: Fixa exporten så att vi har en function typ: ExportItem(string dirPath, IItemExport itemExport),
        //  som exporterar XML:en till en fil i den angivna katalogen, med rätt 'package' path,
        //  och rätt filnamn 

        // NOTE: Svårt att validera innehållet i XML:en i en enhetstest,
        //  vi ha en test som validerar att XML:en innehåller vissa noder/attribut som vi vet.       
        string exportedFilePath = ExportItem.Export("C:\\temp\\Exports", methodExport);


    }
}