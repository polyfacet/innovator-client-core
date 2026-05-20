namespace Users;

public class MemberOfTree
{
    public MemberOfTree(Innovator.Client.IOM.Innovator inn, Item identity)
    {        
        Inn = inn;
        Identity = identity;
        User user = new User(inn);
        List<Item> parentGroups = user.GetGroupsForIdentity(identity);
        foreach(Item parentGroup in parentGroups)
        {
            Parents.Add(new MemberOfTree(inn, parentGroup));
        }
    }

    private Innovator.Client.IOM.Innovator Inn { get; }
    public Item Identity { get;}
    public List<MemberOfTree> Parents { get; } = new List<MemberOfTree>();     
    
}