using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    public GameObject superPowerEffect;
    public GameObject superPowerIndicator;


    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioClip damageSound;
    public AudioClip coinSound;


    [Header("Friends System")]
    public Transform[] friendsPositions;
    public GameObject[] friendsPrefabs;
    public int[] friendCosts;
    private int currentSpawnIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        UpdateUI();
    }

    void Update()
    {
        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 20f : 10f;
        if (Input.GetButtonDown("Fire1"))
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
        }


        if (isSuperPowerReady && Input.GetKey(KeyCode.Space))
        {
            UseSuperPower();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopSuperPower();
        }

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
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        rb.AddForce(direction * pushForce, ForceMode2D.Force);

        superPowerAmount -= 1;
        superPowerBar.fillAmount = (float)superPowerAmount / maxSuperPower;

        if (superPowerAmount <= 0)
        {
            ResetSuperPower();
        }
    }
    void StopSuperPower()
    {
        isUsingSuperPower = false;
        superPowerEffect.SetActive(false);
    }
    void ResetSuperPower()
    {
        isSuperPowerReady = false;
        isUsingSuperPower = false;

        superPowerAmount = 0;
        superPowerBar.fillAmount = 0f;

        superPowerIndicator.SetActive(false);
        superPowerEffect.SetActive(false);
    }




    // ================= FRIEND SPAWN =================

    public void SpawnFriend(int friendIndex)
    {
        if (currentSpawnIndex >= friendsPositions.Length)
            return;

        if (friendIndex < 0 || friendIndex >= friendsPrefabs.Length)
            return;

        int cost = friendCosts[friendIndex];

        if (coinCount < cost)
            return;

        AddCoin(-cost);

        Instantiate(
            friendsPrefabs[friendIndex],
            friendsPositions[currentSpawnIndex].position,
            Quaternion.identity
        );

        currentSpawnIndex++;
    }





    // ================= UI =================

    public void AddCoin(int amount)
    {
        coinCount = Mathf.Max(0, coinCount + amount);
        UpdateUI();
    }

    public void GetDamage(float dmg)
    {
        health -= dmg;
        UpdateUI();
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
            superPowerIndicator.SetActive(true);
        }
    }

}
