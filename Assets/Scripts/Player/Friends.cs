using UnityEngine;
using System.Collections;

public enum FriendState
{
    MoveForward,
    Engage,
    Retreat,
    Reload,
    UseSpecial,
    Dead
}

public class Friends : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    AudioSource audioSource;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float health = 100f;
    public float moveSpeed = 3f;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public float fireCooldown = 1f;
    private float fireTimer;

    [Header("Ammo")]
    public int maxAmmo = 5;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float retreatRange = 3f;

    [Header("SPECIAL POWER (DASH)")]
    public float specialForce = 20f;
    public float specialDamage = 30f;
    public float specialCooldown = 5f;
    public float specialDuration = 0.5f;

    private float specialTimer;
    private bool isUsingSpecial;

    public FriendState currentState = FriendState.MoveForward;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip specialSound;
    public AudioClip deathSound;


    [Header("Engage Vertical Movement")]
    public float verticalMoveAmplitude = 1.5f;
    public float verticalMoveSpeed = 3f;
    private float verticalMoveTime;
    public System.Action<Transform> OnDeath;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentAmmo = maxAmmo;
        fireTimer = 0f;
        specialTimer = 0f;
    }

    void Update()
    {
        if (currentState == FriendState.Dead) return;

        fireTimer -= Time.deltaTime;
        specialTimer -= Time.deltaTime;

        GameObject enemy = FindClosestEnemy();

        if (!isUsingSpecial &&
            health <= maxHealth * 0.7f &&
            specialTimer <= 0f &&
            enemy != null)
        {
            currentState = FriendState.UseSpecial;
        }

        switch (currentState)
        {
            case FriendState.MoveForward:
                MoveForward();
                if (enemy && IsEnemyInRange(enemy))
                    currentState = FriendState.Engage;
                break;

            case FriendState.Engage:
                EngageEnemy(enemy);
                if (enemy && IsEnemyTooClose(enemy))
                    currentState = FriendState.Retreat;
                break;

            case FriendState.Retreat:
                Retreat(enemy);
                break;

            case FriendState.Reload:
                break;

            case FriendState.UseSpecial:
                StartCoroutine(SpecialDash(enemy));
                break;
        }
    }


    void MoveForward()
    {
        // Hedefe doğru hareket et
        GameObject enemy = FindClosestEnemy();
        if (enemy != null)
        {
            Vector2 dir = (enemy.transform.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;
        }
        else
        {
            // Eğer düşman yoksa sağa doğru sabit hareket
            rb.linearVelocity = new Vector2(moveSpeed, 0f);
        }
    }

    void EngageEnemy(GameObject enemy)
    {
        if (enemy == null)
        {
            currentState = FriendState.MoveForward;
            return;
        }

        // MENZİL KORUMA EKLE:
        Vector2 dirToEnemy = enemy.transform.position - transform.position;
        float distance = dirToEnemy.magnitude;

        // İdeal mesafe (5-8 unit arası)
        float minRange = 5f;
        float maxRange = 8f;

        // Mesafeyi ayarla
        if (distance < minRange)
        {
            // Çok yakınsa geri çekil
            Vector2 retreatDir = -dirToEnemy.normalized;
            rb.linearVelocity = retreatDir * moveSpeed;
        }
        else if (distance > maxRange)
        {
            // Çok uzaksa yaklaş
            Vector2 approachDir = dirToEnemy.normalized;
            rb.linearVelocity = approachDir * moveSpeed;
        }
        else
        {
            float yOffset = Mathf.Sin(Time.time * verticalMoveSpeed) * verticalMoveAmplitude;
            rb.linearVelocity = new Vector2(0f, yOffset);
        }

        Vector2 lookDir = dirToEnemy.normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // Ateş et
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
            return;
        }

        if (fireTimer <= 0f)
        {
            Shoot(enemy.transform);
            fireTimer = fireCooldown;
        }
    }



    void Retreat(GameObject enemy)
    {
        if (enemy == null)
        {
            currentState = FriendState.MoveForward;
            return;
        }

        Vector2 dir = (transform.position - enemy.transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }


    IEnumerator SpecialDash(GameObject enemy)
    {
        if (enemy == null) yield break;

        isUsingSpecial = true;
        specialTimer = specialCooldown;
        currentState = FriendState.Engage;
        audioSource.PlayOneShot(specialSound);

        rb.linearVelocity = Vector2.zero;

        // Animasyon
        if (animator != null)
            animator.SetTrigger("Special");

        Vector2 dir = (enemy.transform.position - transform.position).normalized;
        rb.AddForce(dir * specialForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(specialDuration);

        isUsingSpecial = false;
    }


    void Shoot(Transform target)
    {
        currentAmmo--;
        audioSource.PlayOneShot(shootSound);

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Vector2 direction = (target.position - transform.position).normalized;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;

            // Mermiyi yönüne çevir
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        currentState = FriendState.Engage;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isUsingSpecial) return;

        if (collision.gameObject.CompareTag("Enemies") && isUsingSpecial)
        {
            Enemies enemy = collision.gameObject.GetComponent<Enemies>();
            if (enemy != null)
            {
                enemy.GetDamage(specialDamage);
            }
        }
    }


    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }
        return closest;
    }

    bool IsEnemyInRange(GameObject enemy)
    {
        return Vector2.Distance(transform.position, enemy.transform.position) <= detectionRange;
    }

    bool IsEnemyTooClose(GameObject enemy)
    {
        return Vector2.Distance(transform.position, enemy.transform.position) <= retreatRange;
    }


    public void GetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = FriendState.Dead;
            rb.linearVelocity = Vector2.zero;
            audioSource.PlayOneShot(deathSound);

            OnDeath?.Invoke(transform);

            Destroy(gameObject);
        }
    }
}
