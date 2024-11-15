using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shooting : NetworkBehaviour
{
    public GameObject bulletPrefab;       // Prefab untuk peluru
    public Transform bulletSpawnPoint;    // Posisi dari peluru saat ditembakkan
    public float bulletSpeed = 20f;       // Kecepatan peluru saat ditembakkan
    public float bulletLifetime = 2f;     // Waktu hidup peluru sebelum dihancurkan

    void Update()
    {
        // Memeriksa apakah tombol "Fire1" ditekan (biasanya tombol kiri mouse atau Ctrl)
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot(); // Memanggil fungsi menembak
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            // Menciptakan peluru di posisi dan rotasi dari bulletSpawnPoint
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            // Menambahkan gaya pada peluru agar bergerak ke depan
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }

            // Menghancurkan peluru setelah waktu tertentu untuk menghindari penumpukan objek
            Destroy(bullet, bulletLifetime);

            // Menampilkan log untuk mengecek apakah menembak berhasil
            Debug.Log("Menembak peluru dari posisi: " + bulletSpawnPoint.position);
        }
        else
        {
            Debug.LogError("bulletPrefab atau bulletSpawnPoint belum di-assign.");
        }
    }
}
