using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Herramienta para dejar el nivel armado y editable en la escena.
public static class CreateActivity
{
    [MenuItem("Activity/Create Ball Obstacle")]
    public static void Create()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material floor = Material("Platform", new Color(0.65f, 0.65f, 0.65f));
        Material blue = Material("Player", new Color(0.15f, 0.35f, 0.9f));
        Material red = Material("Pillars", new Color(0.9f, 0.2f, 0.2f));
        Material yellow = Material("Collectibles", Color.yellow);
        Material green = Material("Finish", Color.green);

        GameManager game = new GameObject("GameManager").AddComponent<GameManager>();
        Transform platform = new GameObject("Platform").transform;
        Rigidbody platformBody = platform.gameObject.AddComponent<Rigidbody>();
        platformBody.isKinematic = true;
        platformBody.interpolation = RigidbodyInterpolation.Interpolate;
        game.platform = platform.gameObject.AddComponent<PlatformTilt>();

        // Recorrido en U: subir, cruzar el puente y bajar hasta la meta.
        Block("Start platform", platform, new Vector3(-5, 0, -6), new Vector3(5, 0.4f, 5), floor);
        Ramp("Ramp up", platform, new Vector3(-5, 0.6f, -1.5f), -Mathf.Atan2(1.2f, 4f) * Mathf.Rad2Deg, floor);
        Block("Upper left", platform, new Vector3(-5, 1.2f, 3), new Vector3(5, 0.4f, 5), floor);
        Block("Narrow path", platform, new Vector3(0, 1.2f, 3), new Vector3(5.2f, 0.4f, 1.6f), floor);
        Block("Upper right", platform, new Vector3(5, 1.2f, 3), new Vector3(5, 0.4f, 5), floor);
        Ramp("Ramp down", platform, new Vector3(5, 0.6f, -1.5f), -Mathf.Atan2(1.2f, 4f) * Mathf.Rad2Deg, floor);
        Block("Finish platform", platform, new Vector3(5, 0, -6), new Vector3(5, 0.4f, 5), floor);

        game.startPoint = new GameObject("Start point").transform;
        game.startPoint.SetParent(platform, false);
        game.startPoint.localPosition = new Vector3(-5, 0.8f, -7);

        GameObject ball = Primitive("Ball", PrimitiveType.Sphere, null,
            game.startPoint.position, Vector3.one * 0.8f, blue);
        game.ball = ball.AddComponent<Rigidbody>();
        game.ball.mass = 1f;
        game.ball.linearDamping = 1.5f;
        game.ball.interpolation = RigidbodyInterpolation.Interpolate;
        game.ball.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        ball.AddComponent<BallController>().game = game;

        Vector3[] pillarPositions = {
            new Vector3(-5, 2.5f, 2), new Vector3(-3.7f, 2.5f, 4),
            new Vector3(5, 2.5f, 2), new Vector3(6.3f, 2.5f, 4)
        };
        Transform pillars = new GameObject("Pillars").transform;
        for (int i = 0; i < pillarPositions.Length; i++)
        {
            GameObject pillar = Primitive("Pillar " + (i + 1), PrimitiveType.Cylinder,
                pillars, pillarPositions[i], new Vector3(0.65f, 1.1f, 0.65f), red);
            Rigidbody body = pillar.AddComponent<Rigidbody>();
            body.mass = 0.5f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        Vector3[] itemPositions = {
            new Vector3(-5, 0.75f, -4.5f), new Vector3(0, 1.95f, 3),
            new Vector3(5, 1.95f, 4.5f)
        };
        game.collectibles = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            game.collectibles[i] = Block("Collectible " + (i + 1), platform,
                itemPositions[i], Vector3.one * 0.65f, yellow);
            game.collectibles[i].GetComponent<Collider>().isTrigger = true;
        }

        game.finish = Block("Finish", platform, new Vector3(5, 0.4f, -7), new Vector3(2, 0.4f, 1.5f), green);
        game.finish.GetComponent<Collider>().isTrigger = true;

        Camera camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0, 21, -19);
        camera.transform.LookAt(new Vector3(0, 0, -1));
        camera.orthographic = true;
        camera.orthographicSize = 10f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.82f, 0.87f, 0.92f);
        Light light = new GameObject("Directional Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        light.shadows = LightShadows.Soft;
        RenderSettings.ambientLight = new Color(0.65f, 0.65f, 0.65f);

        GameObject canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        var resources = new DefaultControls.Resources {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd")
        };
        Text instructions = Text(canvas.transform, resources, "Instructions", "Flechas: mover | Recoge los 3 cubos amarillos y llega al bloque verde.",
            new Vector2(16, -16), new Vector2(610, 25));
        game.counterText = Text(canvas.transform, resources, "Counter", "Objetos: 0 / 3",
            new Vector2(16, -44), new Vector2(200, 25));
        game.messageText = Text(canvas.transform, resources, "Message", "",
            new Vector2(16, -72), new Vector2(500, 25));
        GameObject button = DefaultControls.CreateButton(resources);
        button.name = "Restart Button";
        button.transform.SetParent(canvas.transform, false);
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-16, -16);
        button.GetComponentInChildren<Text>().text = "Reiniciar";
        UnityEventTools.AddPersistentListener(button.GetComponent<Button>().onClick, game.RestartGame);

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), "Assets/Scenes/BallObstacle.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/BallObstacle.unity", true) };
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = ball;
        if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.FrameSelected();
        Debug.Log("Ball Obstacle: escena creada.");
    }

    static Material Material(string name, Color color)
    {
        const string folder = "Assets/Materials/Activity";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets/Materials", "Activity");
        string path = folder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }
        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    static GameObject Primitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    static GameObject Block(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        return Primitive(name, PrimitiveType.Cube, parent, position, scale, material);
    }

    static void Ramp(string name, Transform parent, Vector3 position, float angle, Material material)
    {
        GameObject ramp = Block(name, parent, position, new Vector3(3, 0.4f, 4.3f), material);
        ramp.transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    static Text Text(Transform parent, DefaultControls.Resources resources, string name, string value, Vector2 position, Vector2 size)
    {
        GameObject obj = DefaultControls.CreateText(resources);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Text text = obj.GetComponent<Text>();
        text.text = value;
        return text;
    }
}
