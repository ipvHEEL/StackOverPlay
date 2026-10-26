using System.Collections.Generic;
using UnityEngine;

// Базовый класс любого здания на сетке (1x1 клетка).
// Чтобы сделать новое здание: унаследуйся, переопредели нужные методы
// и добавь его описание в BuildingDatabase.
public abstract class Building : MonoBehaviour
{
    public BuildingDef Def { get; private set; }
    public Vector2Int Cell { get; private set; }
    public Direction Facing { get; private set; }

    protected WorldGrid World { get { return WorldGrid.Instance; } }

    public void Init(BuildingDef def, Vector2Int cell, Direction facing)
    {
        Def = def;
        Cell = cell;
        transform.position = WorldGrid.CellCenter(cell);
        BuildingVisuals.Create(transform, def, SortingOrders.Building);
        SetFacing(facing);
        OnBuilt();
    }

    public void SetFacing(Direction facing)
    {
        Facing = Def.Rotatable ? facing : Direction.Up;
        transform.rotation = Quaternion.Euler(0f, 0f, Facing.ToAngle());
    }

    protected virtual void OnBuilt() { }

    // Соседнее здание пытается положить в нас предмет. true = приняли.
    public virtual bool TryInsert(ItemType item)
    {
        return false;
    }

    // Отдать всё содержимое (игрок нажал F или здание разобрали).
    public virtual List<ItemStack> TakeContents()
    {
        return new List<ItemStack>();
    }

    // Текст для подсказки при наведении мыши.
    public virtual string GetInfo()
    {
        return Def.Name;
    }
        
    // Клетка, в которую здание выдаёт предметы.
    public Vector2Int OutputCell
    {
        get { return Cell + Facing.ToVector(); }
    }

    // Передать предмет зданию, которое стоит перед нами.
    protected bool PushForward(ItemType item)
    {
        Building target = World.GetBuilding(OutputCell);
        return target != null && target.TryInsert(item);
    }
}

// Общая отрисовка здания (используется и для самих зданий, и для «призрака» при строительстве).
public static class BuildingVisuals
{
    public static List<SpriteRenderer> Create(Transform parent, BuildingDef def, int order)
    {
        var list = new List<SpriteRenderer>();

        var body = new GameObject("Body").AddComponent<SpriteRenderer>();
        body.transform.SetParent(parent, false);
        body.transform.localScale = Vector3.one * def.VisualScale;
        body.sprite = SpriteFactory.Square(def.Color, 0.04f, 1, SpriteFactory.Darker(def.Color));
        body.sortingOrder = order;
        list.Add(body);

        if (def.Rotatable)
        {
            // Маленький квадратик показывает направление выхода.
            var arrow = new GameObject("Arrow").AddComponent<SpriteRenderer>();
            arrow.transform.SetParent(parent, false);
            arrow.transform.localPosition = new Vector3(0f, 0.32f, 0f);
            arrow.transform.localScale = new Vector3(0.3f, 0.15f, 1f);
            arrow.sprite = SpriteFactory.Square(new Color(1f, 0.85f, 0.2f), 0f);
            arrow.sortingOrder = order + 1;
            list.Add(arrow);
        }
        return list;
    }
}
