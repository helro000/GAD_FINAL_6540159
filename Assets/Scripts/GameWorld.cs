using System.Collections.Generic;
using UnityEngine;

/// <summary>Small collection of procedural art helpers. No downloaded art assets are required.</summary>
public static class GameWorld
{
    private static readonly Dictionary<Color32, Material> Materials = new Dictionary<Color32, Material>();

    public static Material GetMaterial(Color color)
    {
        Color32 key = color;
        if (Materials.TryGetValue(key, out Material found)) return found;

        bool srp = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null;
        Shader shader = srp ? Shader.Find("Universal Render Pipeline/Lit") : Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        Material mat = new Material(shader);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        Materials.Add(key, mat);
        return mat;
    }

    public static GameObject Shape(PrimitiveType type, string name, Vector3 position, Vector3 scale,
        Color color, bool collision = true, Transform parent = null)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        if (parent != null) obj.transform.SetParent(parent, true);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = GetMaterial(color);
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null) collider.enabled = collision;
        return obj;
    }

    public static GameObject Part(Transform parent, PrimitiveType type, string name,
        Vector3 localPosition, Vector3 localScale, Color color, bool collision = false)
    {
        GameObject obj = Shape(type, name, Vector3.zero, localScale, color, collision);
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = localScale;
        return obj;
    }

    public static void CylinderBetween(string name, Vector3 a, Vector3 b, float radius, Color color,
        Transform parent = null)
    {
        Vector3 direction = b - a;
        GameObject segment = Shape(PrimitiveType.Cylinder, name, (a + b) * 0.5f,
            new Vector3(radius, direction.magnitude * 0.5f, radius), color, false, parent);
        segment.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
    }

    public static void Ring(string name, Vector3 center, float radius, Color color, int segments = 24)
    {
        GameObject root = new GameObject(name);
        root.transform.position = center;
        for (int i = 0; i < segments; i++)
        {
            float a = 2f * Mathf.PI * i / segments;
            float b = 2f * Mathf.PI * (i + 1) / segments;
            Vector3 p = center + new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0);
            Vector3 q = center + new Vector3(Mathf.Cos(b) * radius, Mathf.Sin(b) * radius, 0);
            CylinderBetween("Ring section", p, q, 0.11f, color, root.transform);
        }
    }

    public static void SunAndCamera(Color sky, Vector3 position, Vector3 lookAt)
    {
        RenderSettings.ambientLight = new Color(0.75f, 0.78f, 0.84f);
        GameObject sun = new GameObject("Sun");
        Light light = sun.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        sun.transform.rotation = Quaternion.Euler(48f, -35f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = sky;
        camera.fieldOfView = 60;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = position;
        cameraObject.transform.LookAt(lookAt);
    }

    public static float HorizontalInput()
    {
        float value = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) value -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) value += 1;
        return value;
    }

    public static float VerticalInput()
    {
        float value = 0;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) value -= 1;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) value += 1;
        return value;
    }
}
