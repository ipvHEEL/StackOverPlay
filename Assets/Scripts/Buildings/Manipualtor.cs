using System.Collections.Generic;
using System.Text;

public class Manipualtor : Building
{
    public float Speed = 2f;
    public int Capacity = 1;
    readonly Inventory storage = new Inventory();

    ItemType item = ItemType.None;

    public override bool TryInsert(ItemType NewItem)
    {
        item = NewItem;
        if (storage.Total >= Capacity) return false;
        storage.Add(item);
        return true;
    }
    
    public override bool PushForward(ItemType item)
    {
        Building target = World.GetBuilding(OutputCell);
        return target != null && target.TryInsert(item);
    }
    private void Update()
    {
        if (item != ItemType.None && Def.Type != BuildingType.Chest)
        {
            PushForward(item);
            item = ItemType.None;
        }
        else 
        {
            return;
        }
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
