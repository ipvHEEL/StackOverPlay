using System.Collections.Generic;
using UnityEngine;

// Конвейер: держит 1 предмет, везёт его от заднего края к переднему
// и передаёт зданию впереди. Принимает предметы с любой стороны.
public class Manipualtor : Building
{
    public float Speed = 2.5f; // клеток в секунду

    public bool returning;

    ItemType item = ItemType.None;
    float progress; // 0 = задний край, 1 = передний край
    SpriteRenderer itemRenderer;
    ManipulatorView view;
    readonly Inventory storage = new Inventory();

    protected override void OnBuilt()
    {
        view = GetComponentInChildren<ManipulatorView>();
        itemRenderer = new GameObject("Item").AddComponent<SpriteRenderer>();

        itemRenderer.transform.SetParent(transform, false);
        itemRenderer.transform.localScale = Vector3.one * 0.45f;
        itemRenderer.sortingOrder = SortingOrders.Item;
        RefreshVisual();
    }

    public override bool TryInsert(ItemType newItem)
    {
        if (returning) return false;
        if (item != ItemType.None) return false;
        item = newItem;
        progress = 0f;
        RefreshVisual();
        return true;
    }

    public void ReturnToStart()
    {
        if (returning)
        { 
            progress = Mathf.MoveTowards(progress, 0f, Speed * Time.deltaTime);
        }

        if (view != null) {
            view.SetProgress(progress);
        }
        if (progress == 0f) {
            returning = false;
        }
        return;
    }

    void Update()
    {
        if (returning)
        {
            ReturnToStart();
            return;
        }
        if (view != null)
            view.SetProgress(progress);
        if (item == ItemType.None) return;

        progress = Mathf.Min(1f, progress + Speed * Time.deltaTime);
        if (progress >= 1f && PushForward(item))
        {
            item = ItemType.None;
            returning = true;
        }
        RefreshVisual();
    }

    public override bool PushForward(ItemType item)
    {
        Building target = World.GetBuilding(OutputCell);
        if (target == null) { return false; }
        return target != null && target.TryInsert(item);
    }

    void RefreshVisual()
    {
        itemRenderer.enabled = item != ItemType.None;
        if (item == ItemType.None) return;
        itemRenderer.sprite = ItemDatabase.GetSprite(item);
        itemRenderer.transform.localPosition = new Vector3(0f, progress - 0.5f, 0f);
    }

    public override List<ItemStack> TakeContents()
    {
        var result = new List<ItemStack>();
        if (item != ItemType.None) result.Add(new ItemStack(item, 1));
        item = ItemType.None;
        RefreshVisual();
        return result;
    }

    public override string GetInfo()
    {
        return Def.Name + "\nВезёт: " + ItemDatabase.GetName(item);
    }
}
