using UnityEngine;

// Игрок: ходьба на WASD/стрелках и ручная добыча руды (зажать ПКМ на руде).
public class PlayerController : MonoBehaviour
{
    public float Speed = 6f;
    public float Reach = 8f;      // дальность взаимодействия в клетках
    public float MiningTime = 0.6f;

    public Inventory Inventory { get; private set; }
    public float MiningProgress { get; private set; } // 0..1, для полоски в HUD

    Vector2Int miningCell;
    Camera cam;

    void Awake()
    {
        Inventory = new Inventory();
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Move();
        Mine();
    }

    void Move()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f) input.Normalize();
        Vector3 delta = input * Speed * Time.deltaTime;

        // По осям отдельно — чтобы можно было скользить вдоль воды.
        var world = WorldGrid.Instance;
        Vector3 pos = transform.position;
        if (world.IsWalkable(WorldGrid.WorldToCell(pos + new Vector3(delta.x, 0f, 0f)))) pos.x += delta.x;
        if (world.IsWalkable(WorldGrid.WorldToCell(pos + new Vector3(0f, delta.y, 0f)))) pos.y += delta.y;
        transform.position = pos;
    }

    void Mine()
    {
        if (!Input.GetMouseButton(1) || HUD.IsMouseOverUI())
        {
            MiningProgress = 0f;
            return;
        }

        var world = WorldGrid.Instance;
        Vector2Int cell = WorldGrid.WorldToCell(cam.ScreenToWorldPoint(Input.mousePosition));
        bool canMine = world.GetBuilding(cell) == null
                       && TileInfo.IsOre(world.GetTile(cell))
                       && InReach(cell);
        if (!canMine)
        {
            MiningProgress = 0f;
            return;
        }

        if (cell != miningCell)
        {
            miningCell = cell;
            MiningProgress = 0f;
        }

        MiningProgress += Time.deltaTime / MiningTime;
        if (MiningProgress >= 1f)
        {
            MiningProgress = 0f;
            ItemType item;
            if (world.TryExtractOre(cell, out item)) Inventory.Add(item);
        }
    }

    public bool InReach(Vector2Int cell)
    {
        return Vector2.Distance(transform.position, WorldGrid.CellCenter(cell)) <= Reach;
    }
}
