using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public float speed = 5.0f;
    public GameObject PlayerModel;
    public Camera playerCamera;  // Referensi ke kamera yang mengikuti pemain

    private void Start()
    {
        // Pastikan kamera mengikuti pemain jika belum diatur
        if (playerCamera == null && isLocalPlayer)
        {
            // Jika kamera belum ditetapkan, cari kamera di scene dan pasangkan
            playerCamera = Camera.main;
            if (playerCamera != null)
            {
                playerCamera.GetComponent<CameraFollow>().player = transform; // Menetapkan pemain untuk kamera mengikuti
            }
        }
    }

    public void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Menggunakan horizontal dan vertical untuk pergerakan
        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);
        transform.position += moveDirection * speed * Time.deltaTime; // Menambahkan Time.deltaTime untuk kecepatan frame-rate independen
    }

    private void Update()
    {
        // Memastikan kode hanya dijalankan pada scene yang tepat
        if(SceneManager.GetActiveScene().name == "Main")
        {
            // Mengaktifkan model pemain jika belum aktif
            if(!PlayerModel.activeSelf)
            {
                SetPosition();
                PlayerModel.SetActive(true);
            }

            // Memeriksa apakah pemain memiliki otoritas
            if(isLocalPlayer)
            {
                Movement();
            }
        }
    }

    public void SetPosition()
    {
        // Menetapkan posisi acak untuk pemain
        transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-15, 7));
    }
}
