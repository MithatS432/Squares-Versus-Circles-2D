using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    [Header("Base Stats")]
    public TextMeshProUGUI enemybasecountText;
    public int enemybasecount = 1000;

    [Header("Attack Settings")]
    public Transform[] firePoints;
    public GameObject bulletPrefab;
    public float bulletSpeed = 25f;
    public float fireRate = 1f;
    private float fireCooldown = 0f;
    public float attackRange = 30f;

    [Header("Audio")]
    public AudioClip shotSound;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        GameObject target = FindClosestTargetInRange();

        if (fireCooldown <= 0f)
        {
            Fire(target);
            int shots = enemybasecount > 500 ? 2 : 5;
            fireCooldown = 1f / shots;
        }
    }

    void Fire(GameObject target)
    {
        if (firePoints.Length == 0 || bulletPrefab == null || target == null)
            return;

        foreach (Transform point in firePoints)
        {
            GameObject bullet = Instantiate(bulletPrefab, point.position, Quaternion.identity);

            Vector2 dir = (target.transform.position - point.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * bulletSpeed;
            }

            if (shotSound != null)
                AudioSource.PlayClipAtPoint(shotSound, point.position);
        }
    }

    GameObject FindClosestTargetInRange()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] friends = GameObject.FindGameObjectsWithTag("Friends");

        GameObject closest = null;
        float minDist = Mathf.Infinity;

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= attackRange)
            {
                minDist = dist;
                closest = player;
            }
        }

        foreach (GameObject f in friends)
        {
            float dist = Vector2.Distance(transform.position, f.transform.position);
            if (dist < minDist && dist <= attackRange)
            {
                minDist = dist;
                closest = f;
            }
        }

        return closest;
    }


    public void GetDamage(int damage)
    {
        enemybasecount -= damage;
        UpdateUI();

        if (enemybasecount <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateUI()
    {
        if (enemybasecountText != null)
            enemybasecountText.text = enemybasecount.ToString();
    }
}
