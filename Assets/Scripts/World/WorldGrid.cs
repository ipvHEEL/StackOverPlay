using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Grass, Sand, Water, Stone, IronOre, CopperOre, Coal }

public static class TileInfo
{
    public static bool IsOre(TileType t)
    {
        return OreItem(t) != ItemType.None;
    }

    public static ItemType OreItem(TileType t)
    {
        switch (t)
        {
            case TileType.IronOre: return ItemType.IronOre;
            case TileType.CopperOre: return ItemType.CopperOre;
            case TileType.Coal: return ItemType.Coal;
            case TileType.Stone: return ItemType.Stone;
            default: return ItemType.None;
        }
    }

    public static bool IsWalkable(TileType t)
    {
        return t != TileType.Water;
    }

    public static bool IsBuildable(TileType t)
    {
        return t != TileType.Water;
    }

    public static Color GetColor(TileType t)
    {
        switch (t)
        {
            case TileType.Grass: return new Color(0.30f, 0.50f, 0.24f);
            case TileType.Sand: return new Color(0.82f, 0.76f, 0.52f);
            case TileType.Water: return new Color(0.18f, 0.36f, 0.70f);
            default: return ItemDatabase.GetColor(OreItem(t));
        }
    }

    public static string GetName(TileType t)
    {
        switch (t)
        {
            case TileType.Grass: return "Трава";
            case TileType.Sand: return "Песок";
            case TileType.Water: return "Вода";
            default: return ItemDatabase.GetName(OreItem(t));
        }
    }
}

// Карта мира: тайлы земли, запасы руды и какие здания где стоят.
// Клетка (x, y) занимает в мире квадрат от (x, y) до (x+1, y+1).
public class WorldGrid : MonoBehaviour
{
    public static WorldGrid Instance { get; private set; }

    public int Width = 128;
    public int Height = 128;
    public int Seed = 0; // 0 = случайный мир

    TileType[,] tiles;
    int[,] oreAmount;
    Building[,] buildings;

    Tilemap tilemap;
    readonly Dictionary<TileType, TileBase> tileAssets = new Dictionary<TileType, TileBase>();

    public Vector3 SpawnPoint
    {
        get { return CellCenter(new Vector2Int(Width / 2, Height / 2)); }
    }

    void Awake()
    {
        Instance = this;
    }

    // ---------- Генерация ----------

    public void Generate()
    {
        if (Seed == 0) Seed = Random.Range(1, 100000);
        var rng = new System.Random(Seed);

        tiles = new TileType[Width, Height];
        oreAmount = new int[Width, Height];
        buildings = new Building[Width, Height];

        float ox = rng.Next(0, 10000);
        float oy = rng.Next(0, 10000);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float h = Mathf.PerlinNoise((x + ox) * 0.045f, (y + oy) * 0.045f);
                if (h < 0.28f) tiles[x, y] = TileType.Water;
                else if (h < 0.33f) tiles[x, y] = TileType.Sand;
                else tiles[x, y] = TileType.Grass;
            }
        }

        ScatterOre(TileType.IronOre, rng);
        ScatterOre(TileType.CopperOre, rng);
        ScatterOre(TileType.Coal, rng);
        ScatterOre(TileType.Stone, rng);

        // Безопасный старт: суша вокруг спавна и по маленькому месторождению каждого типа рядом.
        var c = new Vector2Int(Width / 2, Height / 2);
        Stamp(c, 12, TileType.Grass, 0);
        Stamp(c + new Vector2Int(7, 3), 2, TileType.IronOre, 600);
        Stamp(c + new Vector2Int(-7, 3), 2, TileType.CopperOre, 600);
        Stamp(c + new Vector2Int(3, -7), 2, TileType.Stone, 600);
        Stamp(c + new Vector2Int(-3, -7), 2, TileType.Coal, 600);

        CreateTilemap();
        RedrawAll();
    }

    void ScatterOre(TileType type, System.Random rng)
    {
        float ox = rng.Next(0, 10000);
        float oy = rng.Next(0, 10000);
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (tiles[x, y] != TileType.Grass) continue;
                float n = Mathf.PerlinNoise((x + ox) * 0.08f, (y + oy) * 0.08f);
                if (n > 0.74f)
                {
                    tiles[x, y] = type;
                    oreAmount[x, y] = 200 + (int)((n - 0.74f) * 4000f);
                }
            }
        }
    }

    void Stamp(Vector2Int center, int radius, TileType type, int amount)
    {
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                var p = center + new Vector2Int(x, y);
                if (!InBounds(p) || x * x + y * y > radius * radius) continue;
                tiles[p.x, p.y] = type;
                oreAmount[p.x, p.y] = amount;
            }
        }
    }

    void CreateTilemap()
    {
        var gridGo = new GameObject("Grid");
        gridGo.transform.SetParent(transform, false);
        gridGo.AddComponent<Grid>();

        var tmGo = new GameObject("Ground");
        tmGo.transform.SetParent(gridGo.transform, false);
        tilemap = tmGo.AddComponent<Tilemap>();
        var r = tmGo.AddComponent<TilemapRenderer>();
        r.sortingOrder = SortingOrders.Ground;

        foreach (TileType t in System.Enum.GetValues(typeof(TileType)))
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = SpriteFactory.Square(TileInfo.GetColor(t), TileInfo.IsOre(t) ? 0.15f : 0.05f);
            tileAssets[t] = tile;
        }
    }

    void RedrawAll()
    {
        var positions = new Vector3Int[Width * Height];
        var tileArray = new TileBase[Width * Height];
        int i = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                positions[i] = new Vector3Int(x, y, 0);
                tileArray[i] = tileAssets[tiles[x, y]];
                i++;
            }
        }
        tilemap.SetTiles(positions, tileArray);
    }

    // ---------- Доступ к клеткам ----------

    public bool InBounds(Vector2Int p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < Width && p.y < Height;
    }

    public TileType GetTile(Vector2Int p)
    {
        return InBounds(p) ? tiles[p.x, p.y] : TileType.Water;
    }

    public void SetTile(Vector2Int p, TileType type)
    {
        if (!InBounds(p)) return;
        tiles[p.x, p.y] = type;
        tilemap.SetTile(new Vector3Int(p.x, p.y, 0), tileAssets[type]);
    }

    public int GetOreAmount(Vector2Int p)
    {
        return InBounds(p) ? oreAmount[p.x, p.y] : 0;
    }

    // Достать 1 единицу руды из клетки. Когда руда кончается, клетка становится травой.
    public bool TryExtractOre(Vector2Int p, out ItemType item)
    {
        item = ItemType.None;
        if (!InBounds(p)) return false;
        TileType t = tiles[p.x, p.y];
        if (!TileInfo.IsOre(t) || oreAmount[p.x, p.y] <= 0) return false;

        item = TileInfo.OreItem(t);
        oreAmount[p.x, p.y]--;
        if (oreAmount[p.x, p.y] <= 0) SetTile(p, TileType.Grass);
        return true;
    }

    public bool IsWalkable(Vector2Int p)
    {
        return InBounds(p) && TileInfo.IsWalkable(tiles[p.x, p.y]);
    }

    public Building GetBuilding(Vector2Int p)
    {
        return InBounds(p) ? buildings[p.x, p.y] : null;
    }

    public void SetBuilding(Vector2Int p, Building b)
    {
        if (InBounds(p)) buildings[p.x, p.y] = b;
    }

    // ---------- Координаты ----------

    public static Vector2Int WorldToCell(Vector3 world)
    {
        return new Vector2Int(Mathf.FloorToInt(world.x), Mathf.FloorToInt(world.y));
    }

    public static Vector3 CellCenter(Vector2Int cell)
    {
        return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
    }
}
