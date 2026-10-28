using System.Text;
using UnityEngine;

// Временный интерфейс на OnGUI: инвентарь, панель зданий, подсказки.
// Потом его легко заменить на нормальный UI (Canvas / UI Toolkit).
public class HUD : MonoBehaviour
{
    public PlayerController Player;
    public BuildController Builder;

    const float SlotW = 110f, SlotH = 54f, Gap = 6f;
    static readonly Rect InventoryRect = new Rect(10, 10, 220, 200);

    GUIStyle box, label, slot;

    static Rect HotbarRect()
    {
        int n = BuildingDatabase.All.Length;
        float w = n * SlotW + (n - 1) * Gap;
        return new Rect((Screen.width - w) / 2f, Screen.height - SlotH - 10f, w, SlotH);
    }

    // Курсор над интерфейсом? Тогда клики не должны строить/копать.
    public static bool IsMouseOverUI()
    {
        Vector2 m = Input.mousePosition;
        m.y = Screen.height - m.y;
        return HotbarRect().Contains(m) || InventoryRect.Contains(m);
    }

    void InitStyles()
    {
        if (box != null) return;
        box = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = 14, padding = new RectOffset(8, 8, 6, 6) };
        box.normal.textColor = Color.white;
        label = new GUIStyle(GUI.skin.label) { fontSize = 14 };
        label.normal.textColor = Color.white;
        slot = new GUIStyle(GUI.skin.button) { fontSize = 12, wordWrap = true };
    }

    void OnGUI()
    {
        if (Player == null || Builder == null) return;
        InitStyles();

        DrawInventory();
        DrawHotbar();
        DrawHoverInfo();
        DrawMiningBar();

        GUI.Label(new Rect(10, Screen.height - 110, 420, 100),
            "WASD ходить, колесо зум\nПКМ (зажать) на руде копать вручную\n1-4 выбрать здание, R поворот, Q отмена\nЛКМ построить, ПКМ разобрать, F забрать содержимое",
            label);
    }

    void DrawInventory()
    {
        var sb = new StringBuilder("Инвентарь");
        foreach (ItemType t in System.Enum.GetValues(typeof(ItemType)))
        {
            int n = Player.Inventory.Count(t);
            if (n > 0) sb.Append("\n").Append(ItemDatabase.GetName(t)).Append(": ").Append(n);
        }
        GUI.Box(InventoryRect, sb.ToString(), box);
    }

    void DrawHotbar()
    {
        Rect bar = HotbarRect();
        for (int i = 0; i < BuildingDatabase.All.Length; i++)
        {
            var def = BuildingDatabase.All[i];
            var r = new Rect(bar.x + i * (SlotW + Gap), bar.y, SlotW, SlotH);

            var cost = new StringBuilder();
            foreach (var c in def.Cost) cost.Append(c.Amount).Append(" ").Append(ItemDatabase.GetName(c.Type)).Append("\n");

            GUI.color = Builder.Selected == def ? new Color(1f, 0.85f, 0.3f) : Color.white;
            if (GUI.Button(r, "[" + (i + 1) + "] " + def.Name + "\n" /*+ cost.ToString().TrimEnd()*/, slot))
                Builder.Select(Builder.Selected == def ? null : def);
        }
        GUI.color = Color.white;
    }

    void DrawHoverInfo()
    {
        var world = WorldGrid.Instance;
        Vector2Int cell = Builder.HoverCell;
        if (!world.InBounds(cell)) return;

        TileType t = world.GetTile(cell);
        string text = TileInfo.GetName(t) + "  (" + cell.x + ", " + cell.y + ")";
        if (TileInfo.IsOre(t)) text += "\nЗапас: " + world.GetOreAmount(cell);

        Building b = world.GetBuilding(cell);
        if (b != null) text += "\n\n" + b.GetInfo();
        if (Builder.Selected != null && Builder.LastMessage != null) text += "\n\n<" + Builder.LastMessage + ">";

        GUI.Box(new Rect(Screen.width - 250, 10, 240, 160), text, box);
    }

    void DrawMiningBar()
    {
        if (Player.MiningProgress <= 0f) return;
        Rect bar = HotbarRect();
        var r = new Rect(Screen.width / 2f - 100, bar.y - 24, 200, 12);
        GUI.Box(r, GUIContent.none);
        GUI.color = new Color(1f, 0.8f, 0.2f);
        GUI.DrawTexture(new Rect(r.x + 1, r.y + 1, (r.width - 2) * Player.MiningProgress, r.height - 2), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
