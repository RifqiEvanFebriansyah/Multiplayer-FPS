using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;       // Prefab untuk peluru
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
        if (bulletPrefab != null)
        {
            // Menciptakan peluru di posisi dan rotasi dari GameObject ini
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

            // Menambahkan gaya pada peluru agar bergerak ke depan
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * bulletSpeed;
            }

            // Menghancurkan peluru setelah waktu tertentu untuk menghindari penumpukan objek
            Destroy(bullet, bulletLifetime);

            // Menampilkan log untuk mengecek apakah menembak berhasil
            Debug.Log("Menembak peluru dari posisi: " + transform.position);
        }
        else
        {
            Debug.LogError("bulletPrefab belum di-assign.");
        }
    }
}
