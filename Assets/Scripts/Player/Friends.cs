using UnityEngine;
using System.Collections;

public class Friends : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private AudioSource audioSource;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip deadSound;



    public float speed = 8f;
    public float health = 100f;
    public GameObject bulletPrefab;

    public int maxAmmo = 5;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        currentAmmo = maxAmmo;
    }

    void Update()
    {

    }
    public void GetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            audioSource.PlayOneShot(deadSound);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

    }
}
