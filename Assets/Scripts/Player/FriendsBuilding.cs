using UnityEngine;
using TMPro;
using System.Collections;

public class FriendsBuilding : MonoBehaviour
{
    private Rigidbody2D rb;
    public float health;
    public float bulletSpeed;
    public float fireRate = 1f;
    [SerializeField] private float fireTimer = 1f;

    [SerializeField] private float detectRange = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public AudioClip shootSound;
    public AudioClip destroySound;

    public TextMeshProUGUI friendsBuildingHealthText;
    private GameObject[] enemies;
    private GameObject currentTarget;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        friendsBuildingHealthText.text = health.ToString();
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemies");
        currentTarget = GetClosestEnemy();

        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distance <= detectRange)
        {
            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0f)
            {
                Fire();
                fireTimer = fireRate;
            }
        }
    }
    GameObject GetClosestEnemy()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = e;
            }
        }
        return closest;
    }

    void Fire()
    {
        AudioSource.PlayClipAtPoint(shootSound, transform.position);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        Vector2 direction = (currentTarget.transform.position - firePoint.position).normalized;
        rb.linearVelocity = direction * bulletSpeed;
    }
    public void GetDamage(float damage)
    {
        health -= damage;
        friendsBuildingHealthText.text = health.ToString();
        if (health <= 0)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
            Destroy(gameObject);
        }
    }
}
