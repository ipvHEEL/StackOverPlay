using System.Collections.Generic;
using System.Text;

// Сундук: принимает любые предметы, пока есть место.
public class Chest : Building
{
    public int Capacity = 200;

    readonly Inventory storage = new Inventory();

    public override bool TryInsert(ItemType item)
    {
        if (storage.Total >= Capacity) return false;
        storage.Add(item);
        return true;
    }

    public override List<ItemStack> TakeContents()
    {
        return storage.TakeAll();
    }

    public override string GetInfo()
    {
        var sb = new StringBuilder(Def.Name + " (" + storage.Total + "/" + Capacity + ")");
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
        {
            int n = storage.Count(t);
            if (n > 0) sb.Append("\n").Append(ItemDatabase.GetName(t)).Append(": ").Append(n);
        }
        return sb.ToString();
    }
}
