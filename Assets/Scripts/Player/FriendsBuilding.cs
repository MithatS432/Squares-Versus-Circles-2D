using UnityEngine;
using TMPro;

public class FriendsBuilding : MonoBehaviour
{
    public float health;
    public float bulletSpeed;
    public float fireRate;
    private float detectRange;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public AudioClip shootSound;
    public AudioClip destroySound;

    public TextMeshProUGUI friendsBuildingHealthText;
    void Start()
    {

    }

    void Update()
    {

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
