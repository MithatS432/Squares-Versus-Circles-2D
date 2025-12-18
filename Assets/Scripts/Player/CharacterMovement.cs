using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;


    [Header("Player")]
    public float moveSpeed = 15f;
    public float health = 300f;
    public TextMeshProUGUI healthBar;

    public int coinCount;
    public TextMeshProUGUI coinText;

    public GameObject bulletPrefab;

    public Image superPowerBar;
    public int maxSuperPower = 100;
    private int superPowerAmount = 0;

    public bool isSuperPowerReady = false;
    public bool isUsingSuperPower = false;

    public float pushForce = 40f;
    private float superPowerDamage = 50f;
    public GameObject superPowerEffect;
    private ParticleSystem superPS;

    public GameObject superPowerIndicator;


    public int maxAmmo = 20;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;


    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip damageSound;
    public AudioClip coinSound;
    public AudioClip effectSound;
    public AudioClip lostSound;
    public AudioClip reloudSound;



    [Header("Friends System")]
    public GameObject[] friendsPrefabs;
    public int[] friendCosts;
    private List<Transform> activeSpawnPoints = new List<Transform>();





    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        UpdateUI();
        currentAmmo = maxAmmo;
        superPS = superPowerEffect.GetComponentInChildren<ParticleSystem>();
        superPowerEffect.SetActive(false);

    }

    void Update()
    {
        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 20f : 10f;

        if (isReloading)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }

            Shoot();
        }

        if (isSuperPowerReady && Input.GetKey(KeyCode.Space))
        {
            UseSuperPower();
        }
    }
    void Shoot()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        bullet.GetComponent<BulletFriends>().SetDirection(direction);

        audioSource.PlayOneShot(shootSound);

        currentAmmo--;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        audioSource.PlayOneShot(reloudSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }


    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(x, y) * moveSpeed;
    }

    void UseSuperPower()
    {
        if (!isUsingSuperPower)
        {
            isUsingSuperPower = true;
            superPowerEffect.SetActive(true);
            superPS.Play();

            audioSource.PlayOneShot(effectSound);
            animator.SetTrigger("Power");
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        rb.AddForce(direction * pushForce, ForceMode2D.Force);

        superPowerAmount--;
        superPowerBar.fillAmount = (float)superPowerAmount / maxSuperPower;

        if (superPowerAmount <= 0)
        {
            ResetSuperPower();
        }
    }
    void ResetSuperPower()
    {
        isSuperPowerReady = false;
        isUsingSuperPower = false;

        superPowerAmount = 0;
        superPowerBar.fillAmount = 0f;

        superPowerIndicator.SetActive(false);

        superPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        superPowerEffect.SetActive(false);
    }





    // ================= FRIEND SPAWN =================

    public void SpawnFriend(int friendIndex)
    {
        if (activeSpawnPoints.Count == 0)
            return;

        if (friendIndex < 0 || friendIndex >= friendsPrefabs.Length)
            return;

        int cost = friendCosts[friendIndex];
        if (coinCount < cost)
            return;

        AddCoin(-cost);

        Transform spawnPoint = activeSpawnPoints[0];
        activeSpawnPoints.RemoveAt(0);

        Instantiate(
            friendsPrefabs[friendIndex],
            spawnPoint.position,
            Quaternion.identity
        );
    }

    public void RegisterBarrack(Barrack barrack)
    {
        foreach (Transform t in barrack.spawnPoints)
            activeSpawnPoints.Add(t);
    }

    public void UnregisterBarrack(Barrack barrack)
    {
        foreach (Transform t in barrack.spawnPoints)
            activeSpawnPoints.Remove(t);
    }





    // ================= UI =================

    public void AddCoin(int amount)
    {
        coinCount = Mathf.Max(0, coinCount + amount);
        audioSource.PlayOneShot(coinSound);
        UpdateUI();
    }

    public void GetDamage(float dmg)
    {
        health -= dmg;
        audioSource.PlayOneShot(damageSound);
        UpdateUI();
        if (health <= 0)
        {
            audioSource.PlayOneShot(lostSound);
            Invoke("RestartGame", 2f);
        }
    }
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateUI()
    {
        coinText.text = coinCount.ToString();
        healthBar.text = health.ToString("0");
    }
    public void AddSuperPower(int amount)
    {
        if (isSuperPowerReady) return;

        superPowerAmount += amount;
        superPowerAmount = Mathf.Clamp(superPowerAmount, 0, maxSuperPower);

        superPowerBar.fillAmount = (float)superPowerAmount / maxSuperPower;

        if (superPowerAmount >= maxSuperPower)
        {
            isSuperPowerReady = true;
            animator.SetBool("IsPower", true);
            superPowerIndicator.SetActive(true);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemies") && isUsingSuperPower)
        {
            Enemies enemy = other.gameObject.GetComponent<Enemies>();
            if (enemy != null)
            {
                enemy.GetDamage(superPowerDamage);
            }
        }
    }

}
