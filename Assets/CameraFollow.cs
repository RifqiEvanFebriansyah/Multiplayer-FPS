using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;       // Referensi ke transform pemain
    public float distance = 10f;   // Jarak kamera dari pemain
    public float height = 5f;      // Ketinggian kamera di atas pemain
    public float smoothSpeed = 0.125f;  // Kecepatan lerp untuk gerakan halus

    private void LateUpdate()
    {
        // Memastikan kamera mengikuti pemain
        Vector3 desiredPosition = player.position + new Vector3(0, height, -distance);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Memperbarui posisi kamera
        transform.position = smoothedPosition;

        // Mengarahkan kamera ke pemain
        transform.LookAt(player);
    }
}
