using System;
using System.Collections.Generic;
using UnityEngine;
// Простой инвентарь: предмет -> количество. Используется игроком и сундуками.
public class Inventory
{
    readonly Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

    public event Action Changed;

    public int Count(ItemType type)
    {
        int n;
        return items.TryGetValue(type, out n) ? n : 0;
    }

    public int Total
    {
        get
        {
            int sum = 0;
            foreach (var kv in items) sum += kv.Value;
            return sum;
        }
    }

    public void Add(ItemType type, int amount = 1)
    {
        if (type == ItemType.None || amount <= 0) return;
        items[type] = Count(type) + amount;
        if (Changed != null) Changed();
    }

    public bool Remove(ItemType type, int amount = 1)
    {
        int have = Count(type);
        if (have < amount) return false;
        if (have == amount) items.Remove(type);
        else items[type] = have - amount;
        if (Changed != null) Changed();
        return true;
    }

    public bool HasAll(IList<ItemStack> stacks)
    {
        foreach (var s in stacks)
            if (Count(s.Type) < s.Amount) return false;
        return true;
    }

    
    public bool RemoveAll(IList<ItemStack> stacks)
    {
        if (!HasAll(stacks)) return false;
        foreach (var s in stacks) Remove(s.Type, s.Amount);
        return true;
    }

    public void AddAll(IEnumerable<ItemStack> stacks)
    {
        foreach (var s in stacks) Add(s.Type, s.Amount);
    }



    //public void TryPutToChest()
    //{
        
    //}


    // Забрать всё содержимое (для сундука).
    public List<ItemStack> TakeAll()
    {
        var result = new List<ItemStack>();
        foreach (var kv in items) result.Add(new ItemStack(kv.Key, kv.Value));
        items.Clear();
        if (Changed != null) Changed();
        return result;
    }
}
