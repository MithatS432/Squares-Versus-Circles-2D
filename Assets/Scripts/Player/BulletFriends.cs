using UnityEngine;

public class BulletFriends : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 1.5f;

    private Vector2 direction;
    private LayerMask enemyLayer;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
