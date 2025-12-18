using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float health = 150f;
    void Start()
    {

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
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

    }
}
