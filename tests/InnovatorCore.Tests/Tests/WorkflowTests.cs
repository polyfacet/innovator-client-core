using Innovator.Client.IOM;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine;
using Workflows;

public class WorkflowTests
{
    private readonly ArasFixture _fixture;

    public WorkflowTests(ArasFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_Get_Express_ECO_Workflow_Map() 
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        Item workflowMapItem = inn.GetItemByName("Workflow Map", "Express ECO");
        WorkflowMap workflowMap = new WorkflowMap(inn, workflowMapItem.getID());
        Assert.NotNull(workflowMap);
        Assert.NotEmpty(workflowMap.ActivityTemplates);
        foreach (var item in workflowMap.ActivityTemplates)
        {
            Console.WriteLine(item.Name);
            item.WorkflowMapPaths.ForEach(
                path => 
                Console.WriteLine($"  Path: {path.Name}, To Activity: {path.ToActivity.Name}"));
        }
     }

     [Fact]
    public void Test_Processing_an_Express_ECO()
    {
        // Arrange
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        Item partItem = inn.newItem("Part", "add");
        partItem.setProperty("item_number", "Test Part " + Generators.ShortGUID());
        partItem.setProperty("name", "Test Part");
        partItem = partItem.apply();
        Assert.False(partItem.isError(), $"Error creating part: {partItem.getErrorString()}");
        Item ecoItem = inn.newItem("Express ECO", "add");
        ecoItem.setProperty("title", "Test ECO " + Generators.ShortGUID());
        ecoItem.setProperty("item_number", "Test ECO " + Generators.ShortGUID());
        ecoItem = ecoItem.apply();
        Assert.False(ecoItem.isError(), $"Error creating ECO: {ecoItem.getErrorString()}");

        ECO eco = new ECO(ecoItem);
        Item affectedItem = eco.AddAffectedItem(partItem, "Release");
        Assert.False(affectedItem.isError(), $"Error adding affected item: {affectedItem.getErrorString()}");
               
        // Act/Assert - Process the ECO through its workflow        
        Item signOffResult = eco.SignOff("");
        Assert.False(signOffResult.isError(), $"Error signing off: {signOffResult.getErrorString()}");
        Item claimResult = eco.ClaimSignOffs("Start Work", inn);
        Assert.False(claimResult.isError(), $"Error claiming sign off: {claimResult.getErrorString()}");
        signOffResult = eco.SignOff("Start Work");
        Assert.False(signOffResult.isError(), $"Error signing off: {signOffResult.getErrorString()}");
        claimResult = eco.ClaimSignOffs("Close Change", inn);
        Assert.False(claimResult.isError(), $"Error claiming sign off: {claimResult.getErrorString()}");
        signOffResult = eco.SignOff("Close Change");
        Assert.False(signOffResult.isError(), $"Error signing off: {signOffResult.getErrorString()}");

        string configId = partItem.getProperty("config_id", "N/A");
        Item releasedPart = inn.GetItemByConfigId("Part", configId);
        Assert.False(releasedPart.isError(), $"Error retrieving released part: {releasedPart.getErrorString()}");
        Assert.Equal("Released", releasedPart.getProperty("state", "N/A"));
    }
}