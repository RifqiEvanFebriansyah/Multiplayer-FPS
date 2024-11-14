using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public GameObject powerUpPrefab; // Prefab Power-Up yang akan di-spawn
    public int numberOfPowerUps = 10; // Jumlah power-up yang akan di-spawn
    public float spawnRange = 10f; // Jarak acak untuk spawn power-up
    public float spawnHeight = 1f; // Ketinggian spawn power-up

    void Start()
    {
        // Spawn power-up sebanyak 'numberOfPowerUps'
        SpawnPowerUps();
    }

    // Fungsi untuk men-spawn power-up di posisi acak
    private void SpawnPowerUps()
    {
        for (int i = 0; i < numberOfPowerUps; i++)
        {
            // Membuat posisi acak
            float randomX = Random.Range(-spawnRange, spawnRange);
            float randomZ = Random.Range(-spawnRange, spawnRange);
            Vector3 randomPosition = new Vector3(randomX, spawnHeight, randomZ);

            // Men-spawn power-up di posisi acak
            Instantiate(powerUpPrefab, randomPosition, Quaternion.identity);
        }
    }
}
