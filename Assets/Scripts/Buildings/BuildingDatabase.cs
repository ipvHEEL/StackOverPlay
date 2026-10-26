using System;
using UnityEngine;

public enum BuildingType { Belt, Drill, Furnace, Chest }

// Описание типа здания: имя, цвет, стоимость, какой скрипт вешать.
public class BuildingDef
{
    public BuildingType Type;
    public string Name;
    public Color Color;
    public float VisualScale = 0.92f;
    public bool Rotatable = true;
    public KeyCode Hotkey;
    public ItemStack[] Cost;
    public Type Component; // класс-наследник Building

    // Доп. условие для постройки (например, бур только на руде). null = нет условия.
    public Func<WorldGrid, Vector2Int, bool> ExtraPlacementRule;
    public string PlacementHint;
}

public static class BuildingDatabase
{
    public static readonly BuildingDef[] All =
    {
        new BuildingDef
        {
            Type = BuildingType.Belt,
            Name = "Конвейер",
            Color = new Color(0.25f, 0.25f, 0.28f),
            VisualScale = 0.98f,
            Hotkey = KeyCode.Alpha1,
            Cost = new[] { new ItemStack(ItemType.IronPlate, 1) },
            Component = typeof(ConveyorBelt),
        },
        new BuildingDef
        {
            Type = BuildingType.Drill,
            Name = "Бур",
            Color = new Color(0.85f, 0.70f, 0.15f),
            Hotkey = KeyCode.Alpha2,
            Cost = new[] { new ItemStack(ItemType.IronPlate, 5), new ItemStack(ItemType.Stone, 3) },
            Component = typeof(MiningDrill),
            ExtraPlacementRule = (world, cell) => TileInfo.IsOre(world.GetTile(cell)),
            PlacementHint = "Только на руде",
        },
        new BuildingDef
        {
            Type = BuildingType.Furnace,
            Name = "Печь",
            Color = new Color(0.55f, 0.30f, 0.20f),
            Hotkey = KeyCode.Alpha3,
            Cost = new[] { new ItemStack(ItemType.Stone, 5) },
            Component = typeof(Furnace),
        },
        new BuildingDef
        {
            Type = BuildingType.Chest,
            Name = "Сундук",
            Color = new Color(0.55f, 0.40f, 0.22f),
            VisualScale = 0.8f,
            Rotatable = false,
            Hotkey = KeyCode.Alpha4,
            Cost = new[] { new ItemStack(ItemType.IronPlate, 2) },
            Component = typeof(Chest),
        },
    };

    public static BuildingDef Get(BuildingType type)
    {
        foreach (var d in All)
            if (d.Type == type) return d;
        return null;
    }

    public static bool CanPlace(BuildingDef def, Vector2Int cell, out string reason)
    {
        var world = WorldGrid.Instance;
        reason = null;
        if (!world.InBounds(cell)) { reason = "За краем карты"; return false; }
        if (world.GetBuilding(cell) != null) { reason = "Занято"; return false; }
        if (!TileInfo.IsBuildable(world.GetTile(cell))) { reason = "Нельзя строить здесь"; return false; }
        if (def.ExtraPlacementRule != null && !def.ExtraPlacementRule(world, cell))
        {
            reason = def.PlacementHint;
            return false;
        }
        return true;
    }

    public static Building Spawn(BuildingDef def, Vector2Int cell, Direction facing)
    {
        var go = new GameObject(def.Name);
        go.transform.SetParent(WorldGrid.Instance.transform, false);
        var b = (Building)go.AddComponent(def.Component);
        WorldGrid.Instance.SetBuilding(cell, b);
        b.Init(def, cell, facing);
        return b;
    }

    public static void Despawn(Building b)
    {
        WorldGrid.Instance.SetBuilding(b.Cell, null);
        UnityEngine.Object.Destroy(b.gameObject);
    }
}
