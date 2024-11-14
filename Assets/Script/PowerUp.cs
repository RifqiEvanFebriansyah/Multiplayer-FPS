using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public int healthBoost = 20; // Nilai tambahan darah yang diberikan oleh power-up
    public float spawnRange = 10f; // Jarak acak untuk spawn power-up
    public float spawnHeight = 1f; // Ketinggian spawn power-up

    private void Start()
    {
        // Spawn power-up di posisi acak ketika game dimulai
        SpawnPowerUp();
    }

    // Fungsi untuk men-spawn power-up di posisi acak
    private void SpawnPowerUp()
    {
        // Membuat posisi acak
        float randomX = Random.Range(-spawnRange, spawnRange);
        float randomZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPosition = new Vector3(randomX, spawnHeight, randomZ);

        // Menempatkan power-up di posisi acak
        transform.position = randomPosition;
    }

    // Fungsi yang dipanggil ketika power-up diambil oleh pemain
    private void OnTriggerEnter(Collider other)
    {
        // Mengecek jika objek yang menabrak adalah pemain
        if (other.CompareTag("Player"))
        {
            // Menambah darah pemain
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.AddHealth(healthBoost); // Menambahkan darah
                Destroy(gameObject); // Menghancurkan power-up setelah diambil
            }
        }
    }
}
