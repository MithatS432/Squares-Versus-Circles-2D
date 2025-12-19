using UnityEngine;
using TMPro;

public class EnemyBase : MonoBehaviour
{
    public TextMeshProUGUI enemybasecountText;
    public int enemybasecount = 1000;
    void Start()
    {

    }

    void Update()
    {

    }
    public void GetDamage(int damage)
    {
        enemybasecount -= damage;
        enemybasecountText.text = enemybasecount.ToString();
        if (enemybasecount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
