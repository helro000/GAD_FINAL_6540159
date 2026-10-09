using UnityEngine;
using UnityEngine.SceneManagement;

public enum ThreeInOneScene { MainMenu, Driving, Flying, Sumo }

/// <summary>Scene coordinator, menu navigation, pause and end-of-round behavior.</summary>
public class GameEntry : MonoBehaviour
{
    public ThreeInOneScene scene = ThreeInOneScene.MainMenu;
    private GameUI ui;
    private bool finished;

    public bool IsPaused { get { return ui != null && ui.IsPaused; } }
    public bool IsFinished { get { return finished; } }

    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;
        ui = gameObject.AddComponent<GameUI>();
        ui.Initialize(this);

        switch (scene)
        {
            case ThreeInOneScene.MainMenu:
                SetupMenuWorld();
                ui.ShowMainMenu();
                break;
            case ThreeInOneScene.Driving:
                gameObject.AddComponent<DrivingGame>();
                ui.ShowGameHUD("MAD DRIVER", "W/S accelerate & brake    A/D steer    ESC pause");
                break;
            case ThreeInOneScene.Flying:
                gameObject.AddComponent<FlyingGame>();
                ui.ShowGameHUD("FLY LIKE A BIRD", "W/S climb & descend    A/D move    SPACE boost    ESC pause");
                break;
            case ThreeInOneScene.Sumo:
                gameObject.AddComponent<SumoGame>();
                ui.ShowGameHUD("SUMO BALL", "WASD/Arrows roll    SPACE dash    ESC pause");
                break;
        }
    }

    private void Update()
    {
        if (scene != ThreeInOneScene.MainMenu && !finished && Input.GetKeyDown(KeyCode.Escape))
            ui.TogglePause();
    }

    public void SetStatus(string status) { if (ui != null) ui.SetStatus(status); }

    public void Finish(bool won, string details)
    {
        if (finished) return;
        finished = true;
        ui.ShowResult(won ? "YOU WIN!" : "GAME OVER", details);
        Time.timeScale = 0f;
    }

    public void Open(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void Restart() { Open(SceneManager.GetActiveScene().name); }
    public void BackToMenu() { Open("MainMenu"); }

    public void Quit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetupMenuWorld()
    {
        GameWorld.SunAndCamera(new Color(0.07f, 0.15f, 0.28f),
            new Vector3(0, 8, -19), new Vector3(0, 2, 4));
        GameWorld.Shape(PrimitiveType.Cube, "Display floor", new Vector3(0, -0.8f, 6),
            new Vector3(45, 1, 35), new Color(0.08f, 0.17f, 0.25f));

        GameObject car = GameWorld.Shape(PrimitiveType.Cube, "Preview car", new Vector3(-8, 1, 7),
            new Vector3(3.5f, 1.3f, 6), new Color(0.95f, 0.28f, 0.31f), false);
        GameWorld.Part(car.transform, PrimitiveType.Cube, "Windshield", new Vector3(0, 1f, -0.25f),
            new Vector3(0.85f, 0.8f, 0.55f), new Color(0.24f, 0.74f, 0.92f));
        GameWorld.Shape(PrimitiveType.Sphere, "Preview sumo ball", new Vector3(8, 1.9f, 7),
            new Vector3(3.7f, 3.7f, 3.7f), new Color(1f, 0.69f, 0.2f), false);
        GameWorld.Shape(PrimitiveType.Cube, "Preview plane", new Vector3(0, 4, 12),
            new Vector3(7, 0.3f, 1.8f), new Color(0.22f, 0.89f, 0.85f), false);
    }
}
