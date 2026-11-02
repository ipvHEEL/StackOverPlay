

public class Manipualtor : Building
{

    public int Capacity = 200;

    readonly Inventory storage = new Inventory();
    public override bool TryInsert(ItemType item)
    {
        if (storage.Total >= Capacity) return false;
        storage.Add(item);
        return true;
    }
}
