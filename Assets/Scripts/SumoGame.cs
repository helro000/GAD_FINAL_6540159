using UnityEngine;

/// <summary>Physics sumo: push your opponent out of the circular arena.</summary>
public class SumoGame : MonoBehaviour
{
    private GameEntry game;
    private Rigidbody player;
    private Rigidbody opponent;
    private float dashCooldown;
    private float opponentChargeCooldown;
    private const float ArenaRadius = 14.65f;

    private void Start()
    {
        game = GetComponent<GameEntry>();
        GameWorld.SunAndCamera(new Color(0.29f, 0.49f, 0.72f),
            new Vector3(0, 30, -24), Vector3.zero);
        Camera.main.fieldOfView = 59;
        CreateArena();
        player = CreateFighter("Player sumo ball", new Vector3(-5.5f, 1.3f, -1),
            new Color(0.16f, 0.78f, 0.87f), 2.1f);
        opponent = CreateFighter("Computer opponent", new Vector3(5.5f, 1.3f, 1),
            new Color(1f, 0.43f, 0.21f), 1.65f);
        game.SetStatus("PUSH THE ORANGE BALL OUT!");
    }

    private void CreateArena()
    {
        GameWorld.Shape(PrimitiveType.Cube, "Arena backdrop", new Vector3(0, -5.2f, 0),
            new Vector3(100, 2, 100), new Color(0.09f, 0.18f, 0.24f));
        GameObject platform = GameWorld.Shape(PrimitiveType.Cylinder, "Round sumo platform",
            new Vector3(0, -0.55f, 0), new Vector3(30, 0.55f, 30),
            new Color(0.73f, 0.53f, 0.34f));
        Collider collider = platform.GetComponent<Collider>();
        PhysicsMaterial friction = new PhysicsMaterial("Arena Grip");
        friction.dynamicFriction = 0.9f;
        friction.staticFriction = 0.9f;
        collider.material = friction;

        int segments = 36;
        for (int i = 0; i < segments; i++)
        {
            float a = i * 2f * Mathf.PI / segments;
            float b = (i + 1) * 2f * Mathf.PI / segments;
            Vector3 p = new Vector3(Mathf.Sin(a) * 14.35f, 0.06f, Mathf.Cos(a) * 14.35f);
            Vector3 q = new Vector3(Mathf.Sin(b) * 14.35f, 0.06f, Mathf.Cos(b) * 14.35f);
            GameWorld.CylinderBetween("Ring border", p, q, 0.16f,
                i % 2 == 0 ? new Color(1f, 0.83f, 0.24f) : new Color(0.93f, 0.30f, 0.20f));
        }
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI / 4f;
            Vector3 spot = new Vector3(Mathf.Sin(a) * 9f, 0.036f, Mathf.Cos(a) * 9f);
            GameWorld.Shape(PrimitiveType.Sphere, "Arena indicator", spot,
                new Vector3(0.5f, 0.07f, 0.5f), new Color(0.96f, 0.77f, 0.34f), false);
        }
    }

    private Rigidbody CreateFighter(string name, Vector3 location, Color color, float mass)
    {
        GameObject ball = GameWorld.Shape(PrimitiveType.Sphere, name, location,
            new Vector3(2.2f, 2.2f, 2.2f), color);
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.linearDamping = 1.25f;
        rb.angularDamping = 0.75f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        PhysicsMaterial ballMaterial = new PhysicsMaterial(name + " grip");
        ballMaterial.dynamicFriction = 0.7f;
        ballMaterial.staticFriction = 0.7f;
        ballMaterial.bounciness = 0.1f;
        ball.GetComponent<Collider>().material = ballMaterial;

        GameWorld.Part(ball.transform, PrimitiveType.Sphere, "Team badge", new Vector3(0, 0.5f, -0.91f),
            new Vector3(0.5f, 0.5f, 0.15f), Color.white);
        return rb;
    }

    private void FixedUpdate()
    {
        if (game == null || game.IsFinished || game.IsPaused || player == null) return;

        Vector3 move = new Vector3(GameWorld.HorizontalInput(), 0, GameWorld.VerticalInput());
        if (move.sqrMagnitude > 1) move.Normalize();
        player.AddForce(move * 34f, ForceMode.Acceleration);
        KeepSpeed(player, 11.5f);

        Vector3 chase = player.position - opponent.position;
        chase.y = 0;
        if (chase.sqrMagnitude > 0.05f)
        {
            opponent.AddForce(chase.normalized * 22f, ForceMode.Acceleration);
            KeepSpeed(opponent, 8.7f);
        }

        dashCooldown -= Time.fixedDeltaTime;
        opponentChargeCooldown -= Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.Space) && dashCooldown <= 0 && move.sqrMagnitude > 0.01f)
        {
            player.AddForce(move.normalized * 15f, ForceMode.VelocityChange);
            dashCooldown = 2.1f;
        }
        if (chase.magnitude < 3.0f && opponentChargeCooldown <= 0)
        {
            opponent.AddForce(chase.normalized * 3.5f, ForceMode.VelocityChange);
            opponentChargeCooldown = 2.8f;
        }

        if (OutOfBounds(opponent))
            game.Finish(true, "You pushed the opponent out of the ring!");
        else if (OutOfBounds(player))
            game.Finish(false, "You fell outside the ring. Try again!");
    }

    private bool OutOfBounds(Rigidbody fighter)
    {
        Vector3 p = fighter.position;
        return new Vector2(p.x, p.z).magnitude > ArenaRadius + 0.6f || p.y < -1.8f;
    }

    private void KeepSpeed(Rigidbody fighter, float maxSpeed)
    {
        Vector3 planar = new Vector3(fighter.linearVelocity.x, 0, fighter.linearVelocity.z);
        if (planar.magnitude > maxSpeed)
        {
            Vector3 clamped = planar.normalized * maxSpeed;
            fighter.linearVelocity = new Vector3(clamped.x, fighter.linearVelocity.y, clamped.z);
        }
    }
}
