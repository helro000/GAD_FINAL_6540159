using System.Collections.Generic;
using UnityEngine;

/// <summary>Fly through rings. Collect at least five of eight to complete the course.</summary>
public class FlyingGame : MonoBehaviour
{
    private class FlightRing
    {
        public Vector3 position;
        public GameObject visual;
        public bool passed;
    }

    private readonly List<FlightRing> rings = new List<FlightRing>();
    private GameEntry game;
    private GameObject plane;
    private int collected;
    private int passed;
    private const int RequiredRings = 5;
    private const int TotalRings = 8;

    private void Start()
    {
        game = GetComponent<GameEntry>();
        GameWorld.SunAndCamera(new Color(0.35f, 0.71f, 0.96f),
            new Vector3(0, 13, -18), new Vector3(0, 8, 5));
        BuildEnvironment();
        BuildPlane();
        BuildRings();
        FollowCamera follow = Camera.main.gameObject.AddComponent<FollowCamera>();
        follow.target = plane.transform;
        follow.followRotation = false;
        follow.localOffset = new Vector3(0, 4, -14);
        follow.lookOffset = new Vector3(0, 0.5f, 12);
        follow.smoothing = 3.7f;
        game.SetStatus("RINGS  0 / 8   •   NEED 5");
    }

    private void BuildEnvironment()
    {
        GameWorld.Shape(PrimitiveType.Cube, "Fields below", new Vector3(0, -1, 110),
            new Vector3(220, 1, 270), new Color(0.26f, 0.65f, 0.36f));
        for (int i = 0; i < 18; i++)
        {
            float x = (i % 6 - 2.5f) * 17;
            float z = 12 + (i / 6) * 65;
            GameWorld.Shape(PrimitiveType.Cube, "Patch of farmland", new Vector3(x, -0.43f, z),
                new Vector3(13, 0.06f, 38), i % 2 == 0 ?
                new Color(0.18f, 0.55f, 0.30f) : new Color(0.60f, 0.73f, 0.30f), false);
        }
        for (int i = 0; i < 22; i++)
        {
            float x = ((i * 23) % 100) - 50;
            float z = i * 11;
            Vector3 cloud = new Vector3(x, 22 + (i % 4) * 2, z);
            for (int puff = -1; puff <= 1; puff++)
                GameWorld.Shape(PrimitiveType.Sphere, "Cloud", cloud + new Vector3(puff * 2.1f, 0, 0),
                    new Vector3(5, 2.4f, 3.8f), new Color(0.95f, 0.98f, 1f), false);
        }
    }

    private void BuildPlane()
    {
        plane = new GameObject("Player aircraft");
        plane.transform.position = new Vector3(0, 8, 0);
        GameObject fuselage = GameWorld.Part(plane.transform, PrimitiveType.Capsule, "Fuselage",
            Vector3.zero, new Vector3(0.7f, 1.8f, 0.7f), new Color(0.97f, 0.95f, 0.84f));
        fuselage.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        GameWorld.Part(plane.transform, PrimitiveType.Cube, "Wings", new Vector3(0, 0, -0.3f),
            new Vector3(6.5f, 0.15f, 1.4f), new Color(0.24f, 0.85f, 0.89f));
        GameWorld.Part(plane.transform, PrimitiveType.Cube, "Tail", new Vector3(0, 0.1f, -2.1f),
            new Vector3(2.7f, 0.12f, 0.85f), new Color(0.24f, 0.85f, 0.89f));
        GameWorld.Part(plane.transform, PrimitiveType.Cube, "Tail fin", new Vector3(0, 0.65f, -2.1f),
            new Vector3(0.15f, 1.2f, 0.75f), new Color(0.18f, 0.50f, 0.72f));
        GameWorld.Part(plane.transform, PrimitiveType.Sphere, "Cockpit", new Vector3(0, 0.54f, 0.7f),
            new Vector3(0.75f, 0.55f, 1.4f), new Color(0.14f, 0.43f, 0.73f));
    }

    private void BuildRings()
    {
        float[] xs = { 0, 3, -4, 5, 0, -5, 4, 0 };
        float[] ys = { 8, 10, 12, 9, 7, 11, 9, 8 };
        for (int i = 0; i < TotalRings; i++)
        {
            Vector3 position = new Vector3(xs[i], ys[i], 24 + 21 * i);
            GameWorld.Ring("Checkpoint ring " + (i + 1), position, 3.6f,
                new Color(1f, 0.75f, 0.15f), 22);
            GameObject ringObj = GameObject.Find("Checkpoint ring " + (i + 1));
            rings.Add(new FlightRing { position = position, visual = ringObj });
        }
    }

    private void Update()
    {
        if (game == null || plane == null || game.IsFinished || game.IsPaused) return;

        float moveX = GameWorld.HorizontalInput();
        float moveY = GameWorld.VerticalInput();
        float forwardSpeed = Input.GetKey(KeyCode.Space) ? 22f : 14f;
        Vector3 velocity = new Vector3(moveX * 9.5f, moveY * 8.5f, forwardSpeed);
        plane.transform.position += velocity * Time.deltaTime;
        Vector3 p = plane.transform.position;
        p.x = Mathf.Clamp(p.x, -20f, 20f);
        p.y = Mathf.Clamp(p.y, 1f, 25f);
        plane.transform.position = p;
        plane.transform.rotation = Quaternion.Slerp(plane.transform.rotation,
            Quaternion.Euler(-moveY * 15f, moveX * 10f, -moveX * 24f), 6f * Time.deltaTime);

        foreach (FlightRing ring in rings)
        {
            if (ring.passed || plane.transform.position.z < ring.position.z) continue;
            ring.passed = true;
            passed++;
            float distance = Vector2.Distance(
                new Vector2(plane.transform.position.x, plane.transform.position.y),
                new Vector2(ring.position.x, ring.position.y));
            if (distance < 2.8f)
            {
                collected++;
                if (ring.visual != null) Destroy(ring.visual);
            }
        }

        game.SetStatus("RINGS  " + collected + " / " + TotalRings +
            "     PASSED  " + passed + " / " + TotalRings);

        if (p.y <= 1.01f)
            game.Finish(false, "Your plane flew too low!");
        else if (passed == TotalRings && plane.transform.position.z > rings[TotalRings - 1].position.z + 9)
            game.Finish(collected >= RequiredRings,
                collected >= RequiredRings ? "Great flying! You collected " + collected + " rings." :
                    "You collected " + collected + " rings. Collect at least five to win.");
    }
}
