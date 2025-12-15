using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class CharacterMovement : MonoBehaviour
{
    [Header("Component References")]
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    [Header("Player Settings")]
    public float moveSpeed = 15f;
    private float health = 300f;
    public TextMeshProUGUI healthBar;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }
    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(x, y).normalized;
        rb.linearVelocity = new Vector2(move.x * moveSpeed, move.y * moveSpeed);
    }
    public void GetDamage(float damage)
    {
        health -= damage;
        healthBar.text = health.ToString();
    }
}
