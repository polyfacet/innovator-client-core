using Innovator.Client.IOM;

namespace Users;

public class User : InnovatorBase {
    public User(Innovator.Client.IOM.Innovator inn) : base(inn) {}   
    public Item CreateUser(        
        string loginName, 
        string password,
         string firstName,
        string lastName)
    {
        var user = Inn.newItem("User", "add");
        user.setProperty("login_name", loginName);
        user.setProperty("first_name", firstName);
        user.setProperty("last_name", lastName);
        user.setProperty("password", password);
        var result = user.apply();
        if (result.isError())
        {
            throw new Exception($"Failed to create user: {result.getErrorString()}");
        }
        return result;
    }

    public bool UserExists(string login) {
        Item user = GetUserByLoginName(login);
        if (user.isError()) return false;
        return true;
    }

    public Item GetUserByLoginName(string login)
    {
        Item user = Inn.newItem("User", "get");
        user.setProperty("login_name", login);
        user = user.apply();
        return user;
    }

    public bool IsMemberOf(Item groupIdentity) {
        return IsMemberOf(Identity, groupIdentity, recursive: true);
    }

    public bool IsDirectMemberOf(Item groupIdentity) {
        return IsMemberOf(Identity, groupIdentity, recursive: false);
    }

    private bool IsMemberOf(Item userIdentity, Item groupIdentity, bool recursive) {
        if (userIdentity.getID() == groupIdentity.getID()) return true;

        string aml = $@"<AML>
            <Item action='get' type='Member' select='related_id'>
                <source_id>{groupIdentity.getID()}</source_id>
            </Item>
            </AML>";
        Item relations = Inn.applyAML(aml);
        for (int i = 0; i < relations.getItemCount(); i++)
        {
            Item relatedIdentity = relations.getItemByIndex(i).getRelatedItem();
            if (!relatedIdentity.isError()) {
                string identityId = relatedIdentity.getID();
                if (identityId == userIdentity.getID()) {
                    return true;
                }
            }

            if (recursive) {
                    if (relatedIdentity.getProperty("is_alias") != "1") {
                    // Recurse
                    bool isSubMember = IsMemberOf(userIdentity, relatedIdentity, recursive);
                    if (isSubMember) return true;
                }
            }
            
        }
        return false;
    }

    public static string CreateMD5(string input) {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes); // .NET 5 +
        }
    }

    public Item AddUserAsMember(Item user, string identityNameToAddAsMember) {
        Item member = GetMemberRelation(user, identityNameToAddAsMember);
        if (!member.isError()) return member; // Already added
        
        Item userIdentity = GetIdentity(Inn, user.getID());

        Item parentIdentity = Inn.newItem("Identity", "get");
        parentIdentity.setAttribute("select", "id");
        parentIdentity.setProperty("name", identityNameToAddAsMember);
        parentIdentity = parentIdentity.apply();

        member = Inn.newItem("Member", "add");
        member.setProperty("source_id", parentIdentity.getID());
        member.setProperty("related_id", userIdentity.getID());
        member = member.apply();
        return member;
    }

    public void RemoveUserAsMember(Item user, string identityName)
    {
        Item member = GetMemberRelation(user, identityName);
        if (member.isError()) return; // Not a member
        member.setAction("delete");
        member.apply();
    }

    public Item GetMemberRelation(Item user, string parentIdentityName) {
        Item userIdentity = GetIdentity(Inn, user.getID());
        Item parentIdentity = Inn.newItem("Identity", "get");
        parentIdentity.setAttribute("select", "id");
        parentIdentity.setProperty("name", parentIdentityName);
        parentIdentity = parentIdentity.apply();

        Item member = Inn.newItem("Member", "get");
        member.setAttribute("select", "id");
        member.setAttribute("maxRecords", "1");
        member.setProperty("source_id", parentIdentity.getID());
        member.setProperty("related_id", userIdentity.getID());
        member = member.apply();
        return member;
    }

    public bool UserIsEnabled(string loginName)
    {
        Item user = GetUserByLoginName(loginName);
        bool logonEnabled = (user.getProperty("logon_enabled", "0") == "1") ? true : false;
        return logonEnabled;
    }

    public static Item GetIdentity(Innovator.Client.IOM.Innovator inn, string userId) {
        Item user = inn.newItem("User", "get");
        user.setAttribute("select", "owned_by_id");
        user.setID(userId);
        user = user.apply();
        return user.getPropertyItem("owned_by_id");
    }

    public List<Item> GetGroupsForIdentity(Item userIdentity)
    {
        List<Item> groups = new List<Item>();
        Item memberships = Inn.newItem("Member", "get");
        memberships.setProperty("related_id", userIdentity.getID());
        memberships = memberships.apply();
        if (memberships.isError()) return groups;
        foreach(Item membership in memberships.ToList())
        {
            string parentId = membership.getProperty("source_id");
            Item parentIdentity = Inn.GetItem("Identity", parentId);
            groups.Add(parentIdentity);
        }
        return groups;
    }

    
}