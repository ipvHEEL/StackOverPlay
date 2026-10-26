using System;
using UnityEngine;

// Все предметы игры. Новый предмет = новая строчка здесь + цвет/имя в ItemDatabase.
public enum ItemType
{
    None,
    IronOre,
    CopperOre,
    Coal,
    Stone,
    IronPlate,
    CopperPlate,
    StoneBrick,
    Gun,
}

[Serializable]
public struct ItemStack
{
    public ItemType Type;
    public int Amount;

    public ItemStack(ItemType type, int amount)
    {
        Type = type;
        Amount = amount;
    }
}

public static class ItemDatabase
{
    public static string GetName(ItemType t)
    {
        switch (t)
        {
            case ItemType.IronOre: return "Железная руда";
            case ItemType.CopperOre: return "Медная руда";
            case ItemType.Coal: return "Уголь";
            case ItemType.Stone: return "Камень";
            case ItemType.IronPlate: return "Железная пластина";
            case ItemType.CopperPlate: return "Медная пластина";
            case ItemType.StoneBrick: return "Кирпич";
            case ItemType.Gun: return "Пистолет";
            default: return "-";
        }
    }

    public static Color GetColor(ItemType t)
    {
        switch (t)
        {
            case ItemType.IronOre: return new Color(0.45f, 0.55f, 0.68f);
            case ItemType.CopperOre: return new Color(0.80f, 0.45f, 0.25f);
            case ItemType.Coal: return new Color(0.15f, 0.15f, 0.17f);
            case ItemType.Stone: return new Color(0.62f, 0.56f, 0.45f);
            case ItemType.IronPlate: return new Color(0.80f, 0.84f, 0.90f);
            case ItemType.CopperPlate: return new Color(0.98f, 0.62f, 0.35f);
            case ItemType.StoneBrick: return new Color(0.75f, 0.40f, 0.32f);
            case ItemType.Gun: return new Color(0.30f, 0.30f, 0.34f);
            default: return Color.magenta;
        }
    }

    public static Sprite GetSprite(ItemType t)
    {
        Color c = GetColor(t);
        return SpriteFactory.Square(c, 0.05f, 2, SpriteFactory.Darker(c));
    }

    // Что получается в печи. None = не плавится.
    public static ItemType GetSmeltResult(ItemType t)
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
