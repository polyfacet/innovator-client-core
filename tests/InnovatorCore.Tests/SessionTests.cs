using System.Security.Cryptography;
using System.Text;
using Connections;
using Innovator.Client.IOM;

namespace InnovatorCore.Tests;

public class SessionTests
{
    [Fact]
    public void TestSession()
    {
        Innovator.Client.IOM.Innovator inn = DefaultEnvConfig.GetAdminSession();
        Item identityItemType = inn.newItem("ItemType", "get");
        identityItemType.setProperty("name", "Identity");
        identityItemType = identityItemType.apply();
        Assert.False(identityItemType.isError());
    }

    [Fact]
    public void TestHashSession()
    {
        SessionDTO session = SessionConfig.GetSessionConfig("admin");
        string passwordHash = MakeMD5String(session.Password);
        Innovator.Client.IOM.Innovator inn = Session.CreateSession(session.Url, session.Database, session.Username, passwordHash);
        Assert.True(inn.getConnection().Database == session.Database);
    }

    [Fact]
    public void TestSessionManager()
    {
        SessionDTO session = SessionConfig.GetSessionConfig("admin");
        Innovator.Client.IOM.Innovator inn = SessionManager.CreateSession("admin", session.Url, session.Database, session.Username, session.Password);
        Innovator.Client.IOM.Innovator inn2 = SessionManager.CreateSession("admin2", session.Url, session.Database, session.Username, session.Password);
        Assert.True(inn.getConnection().Database == session.Database);
        Assert.True(inn2.getConnection().Database == session.Database);      
    }

    private static string MakeMD5String(string stringToHash)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(stringToHash);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
        }
    }

}

 