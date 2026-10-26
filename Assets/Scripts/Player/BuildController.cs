using System.Collections.Generic;
using UnityEngine;

// Строительство: выбор здания (1-4), призрак под курсором, R — поворот,
// ЛКМ — поставить (можно зажать и вести), ПКМ — разобрать, F — забрать содержимое.
[RequireComponent(typeof(PlayerController))]
public class BuildController : MonoBehaviour
{
    public BuildingDef Selected { get; private set; }
    public Direction Facing { get; private set; }
    public Vector2Int HoverCell { get; private set; }
    public string LastMessage { get; private set; }

    PlayerController player;
    Camera cam;
    GameObject ghost;
    List<SpriteRenderer> ghostRenderers = new List<SpriteRenderer>();

    void Start()
    {
        player = GetComponent<PlayerController>();
        cam = Camera.main;
    }

    void Update()
    {
        foreach (var def in BuildingDatabase.All)
            if (Input.GetKeyDown(def.Hotkey)) Select(Selected == def ? null : def);
        if (Input.GetKeyDown(KeyCode.Q)) Select(null);

        HoverCell = WorldGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
        Building hovered = WorldGrid.Instance.GetBuilding(HoverCell);

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Selected != null) Facing = Facing.RotateCW();
            else if (hovered != null && hovered.Def.Rotatable) hovered.SetFacing(hovered.Facing.RotateCW());
        }

        UpdateGhost();

        if (HUD.IsMouseOverUI()) return;

        if (Selected != null && Input.GetMouseButton(0)) TryPlace(HoverCell);
        if (Input.GetMouseButtonDown(1) && hovered != null) TryRemove(hovered);
        if (Input.GetKeyDown(KeyCode.F) && hovered != null && player.InReach(HoverCell))
            player.Inventory.AddAll(hovered.TakeContents());
        if (Input.GetKeyDown(KeyCode.G) )//&& hovered != null && player.InReach(HoverCell))
        {
            player.Inventory.TryInsert();
        }

    }



    public void Select(BuildingDef def)
    {
        Selected = def;
        if (ghost != null) Destroy(ghost);
        ghostRenderers.Clear();
        if (def == null) return;

        ghost = new GameObject("Ghost");
        ghostRenderers = BuildingVisuals.Create(ghost.transform, def, SortingOrders.Ghost);
    }

    bool CanPlaceHere(Vector2Int cell, out string reason)
    {
        if (!BuildingDatabase.CanPlace(Selected, cell, out reason)) return false;
        if (!player.InReach(cell)) { reason = "Слишком далеко"; return false; }
        if (!player.Inventory.HasAll(Selected.Cost)) { reason = "Не хватает ресурсов"; return false; }
        return true;
    }

    void UpdateGhost()
    {
        if (ghost == null) return;
        ghost.transform.position = WorldGrid.CellCenter(HoverCell);
        ghost.transform.rotation = Quaternion.Euler(0f, 0f, Selected.Rotatable ? Facing.ToAngle() : 0f);

        string reason;
        bool ok = CanPlaceHere(HoverCell, out reason);
        LastMessage = ok ? null : reason;
        Color tint = ok ? new Color(0.5f, 1f, 0.5f, 0.6f) : new Color(1f, 0.4f, 0.4f, 0.6f);
        foreach (var r in ghostRenderers) r.color = tint;
    }

    void TryPlace(Vector2Int cell)
    {
        string reason;
        if (!CanPlaceHere(cell, out reason)) return;
        player.Inventory.RemoveAll(Selected.Cost);
        BuildingDatabase.Spawn(Selected, cell, Facing);
    }

    void TryRemove(Building b)
    {
        if (!player.InReach(b.Cell)) return;
        player.Inventory.AddAll(b.TakeContents());
        player.Inventory.AddAll(b.Def.Cost); // полный возврат стоимости
        BuildingDatabase.Despawn(b);
    }
}
