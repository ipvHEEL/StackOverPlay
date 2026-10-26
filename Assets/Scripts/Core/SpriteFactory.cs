using System.Collections.Generic;
using UnityEngine;

// Генерирует простые квадратные спрайты в коде, чтобы игра работала без ассетов.
// Когда появятся свои картинки — замени вызовы Square(...) на ссылки на спрайты.
public static class SpriteFactory
{
    public const int Size = 16;

    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    // Квадрат размером ровно 1 клетка (1 юнит) с лёгким шумом и необязательной рамкой.
    public static Sprite Square(Color fill, float noise = 0.06f, int border = 0, Color borderColor = default(Color))
    {
        string key = fill + "|" + noise + "|" + border + "|" + borderColor;
        Sprite sprite;
        if (cache.TryGetValue(key, out sprite)) return sprite;

        var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        var rng = new System.Random(key.GetHashCode());
        var pixels = new Color[Size * Size];
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                bool edge = x < border || y < border || x >= Size - border || y >= Size - border;
                Color c = edge ? borderColor : fill;
                if (!edge && noise > 0f)
                {
                    float n = ((float)rng.NextDouble() * 2f - 1f) * noise;
                    c = new Color(Mathf.Clamp01(c.r + n), Mathf.Clamp01(c.g + n), Mathf.Clamp01(c.b + n), c.a);
                }
                pixels[y * Size + x] = c;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();

        sprite = Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), Size);
        cache[key] = sprite;
        return sprite;
    }

    public static Color Darker(Color c, float k = 0.6f)
    {
        return new Color(c.r * k, c.g * k, c.b * k, c.a);
    }
}

// Порядок отрисовки слоёв.
public static class SortingOrders
{
    public const int Ground = 0;
    public const int Building = 10;
    public const int Item = 20;
    public const int Player = 30;
    public const int Ghost = 40;
}
