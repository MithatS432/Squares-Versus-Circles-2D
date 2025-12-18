using UnityEngine;

public class Barrack : MonoBehaviour
{
    public Transform[] spawnPoints;
    private CharacterMovement player;

    public float health = 300f;

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
    void GetDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
