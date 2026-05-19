using System.Threading;
using Innovator.Client.IOM;

namespace Extensions;
public static class ItemExtensions {

    private const int WaitRetryTimeMs = 100;
    public static Item CreateRelation(this Item item, Item relatedItem, string relationshipName ) {
         Innovator.Client.IOM.Innovator inn = item.getInnovator();
        Item rel = inn.newItem(relationshipName, "add");
        rel.setProperty("source_id", item.getID());
        rel.setProperty("related_id", relatedItem.getID());
        return rel.apply();
    }

    public static Item GetRelations(this Item item, string relationshipName)
    {
        Innovator.Client.IOM.Innovator inn = item.getInnovator();
        Item rels = inn.newItem(relationshipName, "get");
        rels.setProperty("source_id", item.getID());
        return rels.apply();
    }

    public static List<Item> ToList(this Item item)
    {
      if (item.isError()) throw new ApplicationException("Can not convert error item to list");
      List<Item> list = new();
      for (int i = 0; i < item.getItemCount(); i++)
      {
        list.Add(item.getItemByIndex(i));
      }
      return list;
    }
    public static Item Apply(this Item item) {
        Item res = item.apply();
        if (IsDeadLockError(res)) {
          Thread.Sleep(WaitRetryTimeMs);
          res = item.apply();
        }
        return res;       
      }

    public static Item Apply(this Item item, string action) {
        Item res = item.apply(action);
        if (IsDeadLockError(res)) {
          Thread.Sleep(WaitRetryTimeMs);
          res = item.apply(action);
        }
        return res;       
      }

    public static string GetProperty(this Item item, string propertyName) {
        try {
          string value = item.getProperty(propertyName);
          return value;
        }
        catch (Exception ex) {
          Thread.Sleep(WaitRetryTimeMs);
          Console.WriteLine(ex.ToString());
          return item.getProperty(propertyName);
        }
      }

    public static DateTime LastModified(this Item item)
    {
        string lastModifiedStr = item.getProperty("modified_on");
        if (DateTime.TryParse(lastModifiedStr, out DateTime lastModified))
        {
            return lastModified;
        }
        return DateTime.MinValue;
    }

    private static bool IsDeadLockError(Item item) {
        if (!item.isError()) return false;
        if (item.getErrorString().Contains("deadlock victim")) return true;
        return false;
    }

}

