using UnityEngine;
using UnityEngine.UI;

/// <summary>Responsive runtime-created UGUI. All three games share the same pause menu.</summary>
public class GameUI : MonoBehaviour
{
    private GameEntry game;
    private RectTransform canvasRoot;
    private GameObject pauseOverlay;
    private GameObject resultOverlay;
    private Text statusText;
    private bool paused;
    private Font font;

    private readonly Color ink = new Color(0.92f, 0.97f, 1f);
    private readonly Color muted = new Color(0.68f, 0.80f, 0.88f);
    private readonly Color navy = new Color(0.065f, 0.11f, 0.19f, 0.97f);
    private readonly Color aqua = new Color(0.18f, 0.83f, 0.79f);

    public bool IsPaused { get { return paused; } }

    public void Initialize(GameEntry entry)
    {
        game = entry;
#if UNITY_6000_0_OR_NEWER
        try { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { }
        if (font == null) { try { font = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { } }
#else
        try { font = Resources.GetBuiltinResource<Font>("Arial.ttf"); } catch { }
        if (font == null) { try { font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } catch { } }
#endif
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 18);

        GameObject canvas = new GameObject("Game Canvas", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasRoot = canvas.GetComponent<RectTransform>();
        Canvas main = canvas.GetComponent<Canvas>();
        main.renderMode = RenderMode.ScreenSpaceOverlay;
        main.sortingOrder = 100;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject events = new GameObject("UI Event System", typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
            events.transform.SetParent(transform);
        }
    }

    public void ShowMainMenu()
    {
        Panel("Menu panel", canvasRoot, new Vector2(0, 0), new Vector2(685, 800), navy);
        TextLabel("THREE-IN-ONE", canvasRoot, new Vector2(0, 308), new Vector2(650, 95),
            68, ink, TextAnchor.MiddleCenter, FontStyle.Bold);
        TextLabel("Choose your game", canvasRoot, new Vector2(0, 226), new Vector2(600, 55),
            29, muted, TextAnchor.MiddleCenter);
        MenuButton("MAD DRIVER", new Vector2(0, 123), () => game.Open("Driving"));
        MenuButton("FLY LIKE A BIRD", new Vector2(0, 18), () => game.Open("Flying"));
        MenuButton("I'M A SUMO AND A BALL", new Vector2(0, -87), () => game.Open("Sumo"));
        MenuButton("EXIT", new Vector2(0, -192), game.Quit);
        TextLabel("By Your Name   •   Built with Unity", canvasRoot, new Vector2(0, -344),
            new Vector2(580, 40), 21, muted, TextAnchor.MiddleCenter);
    }

    public void ShowGameHUD(string title, string instructions)
    {
        RectTransform header = Panel("HUD Header", canvasRoot, new Vector2(0, 473),
            new Vector2(1800, 115), new Color(0.045f, 0.095f, 0.17f, 0.82f));
        TextLabel(title, header, new Vector2(-590, 10), new Vector2(560, 65),
            38, ink, TextAnchor.MiddleLeft, FontStyle.Bold);
        statusText = TextLabel("Get ready!", header, new Vector2(470, 7), new Vector2(660, 65),
            30, aqua, TextAnchor.MiddleRight, FontStyle.Bold);
        TextLabel(instructions, canvasRoot, new Vector2(0, -495), new Vector2(1800, 55),
            25, ink, TextAnchor.MiddleCenter);
        ButtonAt("II  PAUSE", canvasRoot, new Vector2(765, -393), new Vector2(200, 74),
            new Color(0.11f, 0.26f, 0.35f), TogglePause, 26);
        CreatePauseOverlay();
    }

    public void SetStatus(string content)
    {
        if (statusText != null) statusText.text = content;
    }

    public void TogglePause()
    {
        if (game.IsFinished) return;
        paused = !paused;
        if (pauseOverlay != null) pauseOverlay.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
    }

    private void CreatePauseOverlay()
    {
        RectTransform backdrop = Panel("Pause Overlay", canvasRoot, Vector2.zero,
            new Vector2(1920, 1080), new Color(0.015f, 0.045f, 0.09f, 0.82f));
        pauseOverlay = backdrop.gameObject;
        Panel("Pause Card", backdrop, new Vector2(0, 5), new Vector2(620, 625), navy);
        TextLabel("PAUSED", backdrop, new Vector2(0, 215), new Vector2(570, 100),
            65, ink, TextAnchor.MiddleCenter, FontStyle.Bold);
        ButtonAt("RESUME", backdrop, new Vector2(0, 90), new Vector2(470, 85), aqua,
            TogglePause, 34, true);
        ButtonAt("RESTART", backdrop, new Vector2(0, -25), new Vector2(470, 85),
            new Color(0.13f, 0.38f, 0.50f), game.Restart, 34);
        ButtonAt("BACK TO MAIN MENU", backdrop, new Vector2(0, -140), new Vector2(470, 85),
            new Color(0.13f, 0.38f, 0.50f), game.BackToMenu, 30);
        pauseOverlay.SetActive(false);
    }

    public void ShowResult(string heading, string message)
    {
        RectTransform backdrop = Panel("Round Result Overlay", canvasRoot, Vector2.zero,
            new Vector2(1920, 1080), new Color(0.015f, 0.045f, 0.09f, 0.85f));
        resultOverlay = backdrop.gameObject;
        Panel("Result Card", backdrop, Vector2.zero, new Vector2(760, 550), navy);
        TextLabel(heading, backdrop, new Vector2(0, 161), new Vector2(710, 96),
            64, aqua, TextAnchor.MiddleCenter, FontStyle.Bold);
        TextLabel(message, backdrop, new Vector2(0, 54), new Vector2(680, 93),
            28, ink, TextAnchor.MiddleCenter);
        ButtonAt("PLAY AGAIN", backdrop, new Vector2(0, -65), new Vector2(510, 81),
            aqua, game.Restart, 33, true);
        ButtonAt("MAIN MENU", backdrop, new Vector2(0, -165), new Vector2(510, 81),
            new Color(0.13f, 0.38f, 0.50f), game.BackToMenu, 33);
    }

    private void MenuButton(string text, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        ButtonAt(text, canvasRoot, position, new Vector2(525, 78),
            new Color(0.13f, 0.38f, 0.50f), action, 31);
    }

    private RectTransform Panel(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        obj.GetComponent<Image>().color = color;
        return rect;
    }

    private Text TextLabel(string label, Transform parent, Vector2 pos, Vector2 size,
        int sizeInPoints, Color color, TextAnchor alignment, FontStyle style = FontStyle.Normal)
    {
        GameObject obj = new GameObject("Text: " + label, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        Text text = obj.GetComponent<Text>();
        text.text = label;
        text.font = font;
        text.fontSize = sizeInPoints;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private void ButtonAt(string label, Transform parent, Vector2 pos, Vector2 size, Color color,
        UnityEngine.Events.UnityAction action, int fontSize, bool darkText = false)
    {
        RectTransform background = Panel("Button: " + label, parent, pos, size, color);
        Button button = background.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.83f, 0.98f, 1f);
        colors.pressedColor = new Color(0.63f, 0.86f, 0.92f);
        colors.selectedColor = Color.white;
        button.colors = colors;
        button.onClick.AddListener(action);
        TextLabel(label, background, Vector2.zero, size - new Vector2(22, 4),
            fontSize, darkText ? navy : ink, TextAnchor.MiddleCenter, FontStyle.Bold);
    }
}
