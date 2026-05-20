

using Users;

namespace DemoConsoleApp.Choices;

public class GetMembershipTree : ChoiceBase
{
    public override string Name => "Get Membership Tree";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        List<Item> identitiesToCheck = new List<Item>
        {
            inn.GetIdentity(), // Current user
            inn.GetItemByName("Identity","Super User"),
            inn.GetItemByName("Identity","Change Specialist I"),
            // inn.GetItemByName("Identity","Bruce Wayne")
        };

        foreach (var identity in identitiesToCheck)
        {
            LogLine($"Getting membership tree for identity: [blue]{identity.getProperty("name")}[/]");
            MemberOfTree membershipTree = new MemberOfTree(inn, identity);

            // Log the membership tree
            LogLine("Membership Tree:");
            PrintMembershipTree2(membershipTree);
        }
    }


    private void PrintMembershipTree2(Users.MemberOfTree memberTree)
    {
        Tree tree = BuildConsoleTree(memberTree);
        AnsiConsole.Write(tree);
    }

    private Tree BuildConsoleTree(Users.MemberOfTree memberTree)
    {
        Tree tree = new Tree($"[green]{memberTree.Identity.getProperty("name")}[/]")
            .Style(Style.Parse("green"))
            .Guide(TreeGuide.Line);

        foreach (var parent in memberTree.Parents)
        {
            TreeNode parentNode = tree.AddNode($"[green]{parent.Identity.getProperty("name")}[/]");
            AddParentsToNode(parentNode, parent);
        }
        return tree;
    }

    private TreeNode AddParentsToNode(TreeNode node, Users.MemberOfTree memberTree)
    {
        foreach (var parent in memberTree.Parents)
        {
            TreeNode parentNode = node.AddNode($"[green]{parent.Identity.getProperty("name")}[/]");
            AddParentsToNode(parentNode, parent);
        }
        return node;
    }

}