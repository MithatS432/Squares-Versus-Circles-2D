using UnityEngine;
using TMPro;
public class Barrack : MonoBehaviour
{
    public Transform[] spawnPoints;
    private CharacterMovement player;

    public float health = 300f;
    public TextMeshProUGUI healthBar;

    void Start()
    {
        player = Object.FindFirstObjectByType<CharacterMovement>();
        player.RegisterBarrack(this);
    }

    private void OnDestroy()
    {
        if (player != null)
            player.UnregisterBarrack(this);
    }
    public void GetDamage(float dmg)
    {
        health -= dmg;
        healthBar.text = health.ToString();
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
