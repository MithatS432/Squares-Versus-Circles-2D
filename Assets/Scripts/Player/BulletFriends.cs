using UnityEngine;

public class BulletFriends : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 1.5f;
    private float damage = 15f;

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
        // LayerMask ile karşılaştırma
        if (((1 << other.gameObject.layer) & enemyLayer.value) != 0)
        {
            Destroy(gameObject);

            Enemies enemy = other.GetComponent<Enemies>();
            if (enemy != null)
            {
                enemy.GetDamage((int)damage);
            }

            Outpost outpost = other.GetComponent<Outpost>();
            if (outpost != null)
            {
                outpost.GetDamage((int)damage);
            }

            BarrackEnemy bar = other.GetComponent<BarrackEnemy>();
            if (bar != null)
            {
                bar.GetDamage((int)damage);
            }

            EnemyBase baseenemy = other.GetComponent<EnemyBase>();
            if (baseenemy != null)
            {
                baseenemy.GetDamage((int)damage);
            }
        }
    }
}
