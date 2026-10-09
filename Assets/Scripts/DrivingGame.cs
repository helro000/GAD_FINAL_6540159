using UnityEngine;

public class DrivingHazard : MonoBehaviour { }
public class DrivingFinish : MonoBehaviour { }

/// <summary>Drive along a road, avoid obstacles, and cross the finish gate.</summary>
public class DrivingGame : MonoBehaviour
{
    private GameEntry game;
    private GameObject car;
    private Rigidbody carBody;
    private int lives = 3;
    private float lastHitTime = -10f;
    private const float FinishZ = 190f;

    private void Start()
    {
        game = GetComponent<GameEntry>();
        GameWorld.SunAndCamera(new Color(0.48f, 0.75f, 0.94f),
            new Vector3(0, 8, -12), new Vector3(0, 1, 6));
        CreateRoad();
        CreateCar();
        Camera camera = Camera.main;
        FollowCamera follow = camera.gameObject.AddComponent<FollowCamera>();
        follow.target = car.transform;
        follow.localOffset = new Vector3(0, 7.5f, -14);
        follow.lookOffset = new Vector3(0, 0.8f, 8);
        follow.followRotation = true;
        game.SetStatus("HEALTH  3 / 3     DISTANCE  0 m");
    }

    private void CreateRoad()
    {
        GameWorld.Shape(PrimitiveType.Cube, "Grass", new Vector3(0, -0.45f, 100),
            new Vector3(115, 0.6f, 235), new Color(0.20f, 0.52f, 0.28f));
        GameWorld.Shape(PrimitiveType.Cube, "Asphalt", new Vector3(0, -0.10f, 100),
            new Vector3(21, 0.3f, 220), new Color(0.14f, 0.19f, 0.25f));

        for (int z = 0; z <= 198; z += 12)
        {
            foreach (float x in new float[] { -3.5f, 3.5f })
                GameWorld.Shape(PrimitiveType.Cube, "Road line", new Vector3(x, 0.068f, z),
                    new Vector3(0.18f, 0.025f, 5), Color.white, false);
        }
        for (int i = 0; i < 2; i++)
        {
            float x = i == 0 ? -10.6f : 10.6f;
            GameWorld.Shape(PrimitiveType.Cube, "Safety barrier", new Vector3(x, 0.65f, 100),
                new Vector3(0.8f, 1.3f, 220), new Color(0.86f, 0.89f, 0.95f));
        }

        Vector3[] hazards = new Vector3[]
        {
            new Vector3(-4, 0.8f, 25), new Vector3(5, 0.8f, 42),
            new Vector3(0, 0.8f, 62), new Vector3(-6, 0.8f, 78),
            new Vector3(5, 0.8f, 95), new Vector3(-2, 0.8f, 111),
            new Vector3(4, 0.8f, 128), new Vector3(-5, 0.8f, 145),
            new Vector3(1, 0.8f, 161), new Vector3(6, 0.8f, 173)
        };
        for (int i = 0; i < hazards.Length; i++)
        {
            GameObject block = GameWorld.Shape(PrimitiveType.Cube, "Traffic barrier " + (i + 1),
                hazards[i], new Vector3(2.5f, 1.6f, 1.7f), new Color(1f, 0.46f, 0.16f));
            block.AddComponent<DrivingHazard>();
            GameWorld.Part(block.transform, PrimitiveType.Cube, "Reflective tape", new Vector3(0, 0.12f, -0.52f),
                new Vector3(0.85f, 0.26f, 0.08f), Color.white);
        }

        GameObject finish = new GameObject("Finish line trigger");
        finish.transform.position = new Vector3(0, 2.2f, FinishZ);
        BoxCollider trigger = finish.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(19, 5, 2.2f);
        finish.AddComponent<DrivingFinish>();
        GameWorld.Shape(PrimitiveType.Cube, "Finish stripe", new Vector3(0, 0.08f, FinishZ),
            new Vector3(20, 0.045f, 2), new Color(0.98f, 0.87f, 0.21f), false);
        for (int i = -1; i <= 1; i += 2)
            GameWorld.Shape(PrimitiveType.Cube, "Finish post", new Vector3(i * 9.7f, 3.3f, FinishZ),
                new Vector3(0.7f, 6.6f, 0.7f), new Color(1f, 0.86f, 0.22f), false);
        GameWorld.Shape(PrimitiveType.Cube, "Finish sign", new Vector3(0, 6.5f, FinishZ),
            new Vector3(20, 1, 0.8f), new Color(0.98f, 0.86f, 0.23f), false);

        for (int i = 0; i < 26; i++)
        {
            float z = 5 + i * 8;
            float side = (i % 2 == 0) ? 1 : -1;
            GameWorld.Shape(PrimitiveType.Cylinder, "Roadside tree trunk", new Vector3(side * 19, 1.5f, z),
                new Vector3(0.40f, 1.5f, 0.40f), new Color(0.36f, 0.25f, 0.17f), false);
            GameWorld.Shape(PrimitiveType.Sphere, "Roadside tree leaves", new Vector3(side * 19, 4, z),
                new Vector3(3, 3, 3), new Color(0.12f, 0.41f, 0.21f), false);
        }
    }

    private void CreateCar()
    {
        car = new GameObject("Player car");
        car.transform.position = new Vector3(0, 1.05f, 0);
        BoxCollider collider = car.AddComponent<BoxCollider>();
        collider.size = new Vector3(1.95f, 1.5f, 3.6f);
        carBody = car.AddComponent<Rigidbody>();
        carBody.mass = 2f;
        carBody.linearDamping = 0.8f;
        carBody.angularDamping = 3f;
        carBody.interpolation = RigidbodyInterpolation.Interpolate;
        carBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        carBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        GameWorld.Part(car.transform, PrimitiveType.Cube, "Red body", Vector3.zero,
            new Vector3(1.9f, 0.8f, 3.6f), new Color(0.93f, 0.18f, 0.25f));
        GameWorld.Part(car.transform, PrimitiveType.Cube, "Roof", new Vector3(0, 0.63f, -0.32f),
            new Vector3(1.55f, 0.7f, 1.8f), new Color(0.92f, 0.2f, 0.27f));
        GameWorld.Part(car.transform, PrimitiveType.Cube, "Front window", new Vector3(0, 0.65f, 0.61f),
            new Vector3(1.45f, 0.51f, 0.14f), new Color(0.23f, 0.72f, 0.86f));
        for (int side = -1; side <= 1; side += 2)
        {
            for (int axle = -1; axle <= 1; axle += 2)
            {
                GameObject wheel = GameWorld.Part(car.transform, PrimitiveType.Cylinder, "Wheel",
                    new Vector3(side * 1.03f, -0.36f, axle * 1.16f),
                    new Vector3(0.47f, 0.19f, 0.47f), new Color(0.09f, 0.12f, 0.17f));
                wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            GameWorld.Part(car.transform, PrimitiveType.Cube, "Headlight",
                new Vector3(side * 0.55f, -0.06f, 1.82f),
                new Vector3(0.4f, 0.25f, 0.08f), new Color(1f, 0.95f, 0.59f));
        }
        car.AddComponent<DrivingCollision>().Initialize(this);
    }

    private void FixedUpdate()
    {
        if (game == null || game.IsFinished || game.IsPaused || carBody == null) return;
        float vertical = GameWorld.VerticalInput();
        float horizontal = GameWorld.HorizontalInput();
        float forwardSpeed = Vector3.Dot(carBody.linearVelocity, car.transform.forward);
        float desiredSpeed = vertical > 0 ? 24f : vertical < 0 ? -9f : 0f;
        float speed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, 19f * Time.fixedDeltaTime);
        float turningFactor = Mathf.Clamp01(Mathf.Abs(speed) / 6f);
        car.transform.Rotate(0, horizontal * 82f * turningFactor * Mathf.Sign(speed) * Time.fixedDeltaTime, 0);
        Vector3 travel = car.transform.forward * speed;
        carBody.linearVelocity = new Vector3(travel.x, carBody.linearVelocity.y, travel.z);
        if (car.transform.position.y < -6) game.Finish(false, "Your car left the road.");
    }

    private void Update()
    {
        if (game == null || car == null || game.IsFinished) return;
        game.SetStatus("HEALTH  " + lives + " / 3     DISTANCE  " +
            Mathf.Clamp(Mathf.RoundToInt(car.transform.position.z), 0, (int)FinishZ) + " / 190 m");
    }

    public void HitBarrier()
    {
        if (game.IsFinished || Time.time - lastHitTime < 1.5f) return;
        lastHitTime = Time.time;
        lives--;
        carBody.linearVelocity *= 0.22f;
        if (lives <= 0) game.Finish(false, "You hit too many barriers. Try again!");
    }

    public void CrossFinish() { game.Finish(true, "You reached the finish line!"); }
}

public class DrivingCollision : MonoBehaviour
{
    private DrivingGame driver;
    public void Initialize(DrivingGame owner) { driver = owner; }

    private void OnCollisionEnter(Collision collision)
    {
        if (driver != null && collision.gameObject.GetComponent<DrivingHazard>() != null)
            driver.HitBarrier();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (driver != null && other.GetComponent<DrivingFinish>() != null)
            driver.CrossFinish();
    }
}
