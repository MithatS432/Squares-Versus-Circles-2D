using UnityEngine;

public class Enemies : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource audioSource;

    [Header("Stats")]
    public float health = 150f;
    public float moveSpeed = 3f;
    public int superPowerScore = 10;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public float attackRange = 8f;
    public float fireCooldown = 1.2f;

    float fireTimer;
    Transform target;

    // AI hareketi için
    float verticalAmplitude = 1f; // Yukarı-aşağı hareket mesafesi
    float verticalSpeed = 2f;     // Hareket hızı
    Vector2 initialPosition;

    public int addCoin;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip deathSound;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        fireTimer = fireCooldown;
        initialPosition = transform.position;
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;
        target = FindClosestTarget();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            MoveLeftWithVertical();
            return;
        }

        float dist = Vector2.Distance(transform.position, target.position);

        if (dist > attackRange)
        {
            MoveLeftWithVertical();
        }
        else
        {
            AttackBehavior();
        }
    }

    void MoveLeftWithVertical()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * verticalSpeed) * verticalAmplitude;
        rb.linearVelocity = new Vector2(-moveSpeed, (newY - transform.position.y) / Time.fixedDeltaTime);
    }

    void AttackBehavior()
    {
        float dodgeY = Mathf.Sin(Time.time * verticalSpeed) * 0.5f;
        rb.linearVelocity = new Vector2(0f, dodgeY);

        if (fireTimer <= 0f)
        {
            Shoot(target);
            fireTimer = fireCooldown;
        }
    }

    void Shoot(Transform t)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );
        audioSource.PlayOneShot(shootSound);
        EnemiesBullet b = bullet.GetComponent<EnemiesBullet>();
        if (b != null)
        {
            Vector2 dir = (t.position - transform.position).normalized;
            b.SetDirection(dir);
        }
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            CharacterMovement player = FindFirstObjectByType<CharacterMovement>();
            if (player != null)
            {
                player.AddSuperPower(superPowerScore);
                player.AddCoin(addCoin);
            }

            audioSource.PlayOneShot(deathSound);

            Destroy(gameObject);
        }
    }

    Transform FindClosestTarget()
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        Friends[] friends = FindObjectsByType<Friends>(FindObjectsSortMode.None);
        foreach (Friends f in friends)
        {
            float d = Vector2.Distance(transform.position, f.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = f.transform;
            }
        }

        FriendsBuilding[] friendsBuildings = FindObjectsByType<FriendsBuilding>(FindObjectsSortMode.None);
        foreach (FriendsBuilding f in friendsBuildings)
        {
            float d = Vector2.Distance(transform.position, f.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = f.transform;
            }
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            float d = Vector2.Distance(transform.position, playerObj.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = playerObj.transform;
            }
        }

        return closest;
    }
}
