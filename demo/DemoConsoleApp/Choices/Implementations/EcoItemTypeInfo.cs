
using Extensions;
using Workflows;

namespace DemoConsoleApp.Choices;

public class EcoItemTypeInfo : ChoiceBase
{
    public override string Name => "Eco Item Type Info";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        ItemType ecoItemType = inn.DataModel().ItemType("Express ECO");
        LogLine($"Is Workflow Enabled: [blue]{ecoItemType.IsWorkflowEnabled().ToString()}[/]");
        LogLine($"ECO ItemType last modified: [blue]{ecoItemType.Item.LastModified()}[/]");

        foreach (var workflowMap in ecoItemType.WorkflowMaps())
        {
            workflowMap.WorkflowMapItem.getProperty("name");
            LogLine($"Workflow Map: [blue]{workflowMap.WorkflowMapItem.getProperty("name")}[/]");
            
            HashSet<Workflows.ActivityTemplate> activitiesAdded = new HashSet<Workflows.ActivityTemplate>();


            List<Workflows.ActivityTemplate> allActivities = workflowMap.ActivityTemplates.ToList();
            LogLine($"Activities remaining to add: [yellow]{allActivities.Count}[/]");
            // Start activity
            Workflows.ActivityTemplate? startActivity = workflowMap.ActivityTemplates.ToList().FirstOrDefault(t => t.IsStart);
            if (startActivity != null)
            {
                LogLine($"  Is Start Activity: [blue]{startActivity.Name}[/]");
                foreach (var path in startActivity.WorkflowMapPaths)
                {
                    LogLine($"    Workflow Map Path: [blue]{path.Name}[/], to Activity Template: [blue]{path.ToActivity.Name}[/]");
                }
                activitiesAdded.Add(startActivity);
                allActivities.Remove(startActivity);                
            }

            // Add activities after added activities
            int maxLoops = 20; // Prevent infinite loop in case of circular paths
            int loopCount = 0;
            while(allActivities.Count > 0 && maxLoops > loopCount) {
                loopCount++;
                List<Workflows.ActivityTemplate> activitiesToAdd = GetPossibleEligibleActivities(allActivities, activitiesAdded);
                foreach (var activityTemplate in activitiesToAdd)
                {
                    LogLine($"  Activity Template: [blue]{activityTemplate.Name}[/]");
                    foreach (var path in activityTemplate.WorkflowMapPaths)
                    {
                        LogLine($"    Workflow Map Path: [blue]{path.Name}[/], to Activity Template: [blue]{path.ToActivity.Name}[/]");
                    }
                    activitiesAdded.Add(activityTemplate);
                    allActivities.Remove(activityTemplate);
                }
            }
        }
    }

    private List<ActivityTemplate> GetPossibleEligibleActivities(List<ActivityTemplate> allActivities, HashSet<ActivityTemplate> activitiesAdded)
    {
        List<string> eligibleActivityNames = new List<string>();
        foreach (var activity in activitiesAdded)
        {
            foreach (var path in activity.WorkflowMapPaths) {
                eligibleActivityNames.Add(path.ToActivity.Name.ToString());
            }
        }

        var list = new List<ActivityTemplate>();
        foreach (var activity in allActivities)
        {
            if (eligibleActivityNames.Contains(activity.Name))
            {
                list.Add(activity);
            }
        }
        return list;
    }
}