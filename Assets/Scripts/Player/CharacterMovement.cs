using UnityEngine;
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
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(shootSound);
            Destroy(bullet, 1f);
        }
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(x, y) * moveSpeed;
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
}
