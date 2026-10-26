using System.Collections.Generic;
using UnityEngine;

// Печь: принимает руду, переплавляет (см. ItemDatabase.GetSmeltResult)
// и выдаёт результат вперёд. Топливо пока не нужно — хорошее первое улучшение.
public class Furnace : Building
{
    public float SmeltTime = 2f;
    public int MaxStack = 20;

    ItemType inputType = ItemType.None;
    int inputCount;
    ItemType outputType = ItemType.None;
    int outputCount;
    float progress;

    public override bool TryInsert(ItemType item)
    {
        if (ItemDatabase.GetSmeltResult(item) == ItemType.None) return false;
        if (inputCount > 0 && inputType != item) return false;
        if (inputCount >= MaxStack) return false;
        inputType = item;
        inputCount++;
        return true;
    }

    void Update()
    {
        if (inputCount > 0)
        {
            ItemType result = ItemDatabase.GetSmeltResult(inputType);
            bool hasRoom = outputCount == 0 || (outputType == result && outputCount < MaxStack);
            if (hasRoom)
            {
                progress += Time.deltaTime / SmeltTime;
                if (progress >= 1f)
                {
                    progress = 0f;
                    inputCount--;
                    outputType = result;
                    outputCount++;
                }
            }
        }

        if (outputCount > 0 && PushForward(outputType))
            outputCount--;
    }

    public override List<ItemStack> TakeContents()
    {
        var result = new List<ItemStack>();
        if (inputCount > 0) result.Add(new ItemStack(inputType, inputCount));
        if (outputCount > 0) result.Add(new ItemStack(outputType, outputCount));
        inputCount = 0;
        outputCount = 0;
        progress = 0f;
        return result;
    }

    public override string GetInfo()
    {
        return Def.Name +
               "\nВход: " + ItemDatabase.GetName(inputType) + " x" + inputCount +
               "\nВыход: " + ItemDatabase.GetName(outputType) + " x" + outputCount +
               "\nПрогресс: " + Mathf.RoundToInt(progress * 100) + "%";
    }
}
