using System.Collections.Generic;
using UnityEngine;

// Бур: ставится на руду, раз в Interval секунд добывает 1 руду
// и выдаёт её в клетку перед собой (на конвейер, в печь, в сундук).
public class MiningDrill : Building
{
    public float Interval = 1.5f;

    float timer;
    ItemType output = ItemType.None; // добытое, но ещё не отданное

    void Update()
    {
        if (output == ItemType.None)
        {
            timer += Time.deltaTime;
            if (timer >= Interval)
            {
                timer = 0f;
                World.TryExtractOre(Cell, out output);
            }
        }

        if (output != ItemType.None && PushForward(output))
            output = ItemType.None;
    }

    public override List<ItemStack> TakeContents()
    {
        var result = new List<ItemStack>();
        if (output != ItemType.None) result.Add(new ItemStack(output, 1));
        output = ItemType.None;
        return result;
    }

    public override string GetInfo()
    {
        int left = World.GetOreAmount(Cell);
        string status = left > 0 ? "Осталось руды: " + left : "Руда закончилась";
        if (output != ItemType.None && World.GetBuilding(OutputCell) == null) status += "\nНекуда выгружать!";
        return Def.Name + "\n" + status;
    }
}
