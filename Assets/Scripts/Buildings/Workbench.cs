using System.Collections.Generic;
using UnityEngine;




public class Workbench : Building
{
    public float CraftTime = 2f;



    public static ItemType GetItemCraft(ItemType t) // nothing
    {
        switch (t)
        {
            case ItemType.IronOre: return ItemType.IronPlate;
            case ItemType.CopperOre: return ItemType.CopperPlate;
            case ItemType.Stone: return ItemType.StoneBrick;
            default: return ItemType.None;
        }
    }
}
