using UnityEngine;
using System.Collections.Generic;

public class Outpost : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float detectionRange = 10f;
    public float fireRate = 1f;

    private float fireCooldown = 0f;
    public float health = 250f;

    public AudioClip destroySound;
    public AudioClip shotSound;

    private List<Transform> targets = new List<Transform>();

    void Start()
    {
        // Başlangıçta sahnedeki Player ve Friends objelerini tag ile bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            targets.Add(playerObj.transform);

        GameObject[] friendsObjs = GameObject.FindGameObjectsWithTag("Friends");
        foreach (GameObject friend in friendsObjs)
        {
            targets.Add(friend.transform);
        }
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        targets.Clear();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            targets.Add(playerObj.transform);

        GameObject[] friendsObjs = GameObject.FindGameObjectsWithTag("Friends");
        foreach (GameObject friend in friendsObjs)
            targets.Add(friend.transform);

        // Menzil kontrolü ve ateşleme
        Transform target = null;
        foreach (Transform t in targets)
        {
            if (t != null && Vector3.Distance(transform.position, t.position) <= detectionRange)
            {
                target = t;
                break;
            }
        }

        if (target != null && fireCooldown <= 0f)
        {
            Fire(target);
            fireCooldown = 1f / fireRate;
        }
    }


    void Fire(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(shotSound, transform.position);

        Vector3 direction = (target.position - firePoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 20f;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }
    }
}
