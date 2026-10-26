using UnityEngine;

// Точка входа. Собирает всю игру из кода при нажатии Play в любой сцене —
// ничего руками на сцену добавлять не нужно. Хочешь собирать сцену сам —
// поставь AutoStart = false.
public static class GameBootstrap
{
    public static bool AutoStart = true;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (!AutoStart || WorldGrid.Instance != null) return;

        // Мир
        var world = new GameObject("World").AddComponent<WorldGrid>();
        world.Generate();

        // Игрок
        var playerGo = new GameObject("Player");
        playerGo.transform.position = world.SpawnPoint;
        var body = new GameObject("Body").AddComponent<SpriteRenderer>();
        body.transform.SetParent(playerGo.transform, false);
        body.transform.localScale = Vector3.one * 0.7f;
        var playerColor = new Color(1f, 0.55f, 0.1f);
        body.sprite = SpriteFactory.Square(playerColor, 0.03f, 2, SpriteFactory.Darker(playerColor, 0.4f));
        body.sortingOrder = SortingOrders.Player;

        var player = playerGo.AddComponent<PlayerController>();
        var builder = playerGo.AddComponent<BuildController>();

        // Стартовые ресурсы, чтобы сразу можно было что-то построить.
        player.Inventory.Add(ItemType.IronPlate, 50);
        player.Inventory.Add(ItemType.Stone, 30);

        // Камера
        Camera cam = Camera.main;
        if (cam == null)
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 8f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.1f);
        cam.transform.position = new Vector3(world.SpawnPoint.x, world.SpawnPoint.y, -10f);

        var follow = cam.GetComponent<CameraFollow>();
        if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();
        follow.Target = playerGo.transform;

        // Интерфейс
        var hud = new GameObject("HUD").AddComponent<HUD>();
        hud.Player = player;
        hud.Builder = builder;
    }
}
