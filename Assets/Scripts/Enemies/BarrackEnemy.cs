using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarrackEnemy : MonoBehaviour
{
    public GameObject[] enemyPrefabs;    // Farklı düşman tipleri (4 adet)
    public Transform[] spawnPoints;      // Spawn noktaları
    public float spawnInterval = 5f;     // Spawn aralığı (saniye)
    public int minEnemies = 1;           // Minimum spawn sayısı
    public int maxEnemies = 4;           // Maximum spawn sayısı

    public float health = 200f;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

            // Spawn noktalarını karıştır
            List<Transform> availablePoints = new List<Transform>(spawnPoints);

            for (int i = 0; i < enemyCount && availablePoints.Count > 0; i++)
            {
                // Rastgele spawn noktası seç
                int pointIndex = Random.Range(0, availablePoints.Count);
                Transform spawnPoint = availablePoints[pointIndex];
                availablePoints.RemoveAt(pointIndex);

                // Rastgele enemy tipi seç
                int enemyIndex = Random.Range(0, enemyPrefabs.Length);
                GameObject enemyPrefab = enemyPrefabs[enemyIndex];

                // Instantiate et
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void GetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
