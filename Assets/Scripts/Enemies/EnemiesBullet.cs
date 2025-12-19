using UnityEngine;

public class EnemiesBullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 1.5f;
    public float damage = 15f;

    private Vector2 direction;
    private LayerMask friendLayer;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
        friendLayer = LayerMask.GetMask("Friend");
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & friendLayer) != 0)
        {
            // Friends
            Friends friend = other.GetComponent<Friends>();
            if (friend != null)
                friend.GetDamage(damage);

            CharacterMovement player = other.GetComponent<CharacterMovement>();
            if (player != null)
                player.GetDamage(damage);

            Barrack barrack = other.GetComponent<Barrack>();
            if (barrack != null)
                barrack.GetDamage(damage);

            FriendsBuilding build = other.GetComponent<FriendsBuilding>();
            if (build != null)
                build.GetDamage(damage);

            Destroy(gameObject);
        }
    }
}
