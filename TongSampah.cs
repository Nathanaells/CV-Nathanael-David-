using UnityEngine;

public class TrashCanMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Kecepatan gerak tong sampah
    public float moveDistance = 2f; // Jarak maksimum gerakan tong sampah dari posisi awal
    public float offsetY = 1.5f; // Nilai offset dari posisi kamera untuk menentukan posisi Y tong sampah
    public float targetY = -1f; // Nilai Y target untuk posisi tong sampah

    private Vector3 initialPosition; // Posisi awal tong sampah


    void Start()
    {
        // Mendapatkan posisi kamera utama
        Vector3 cameraPosition = Camera.main.transform.position;

        // Mengatur posisi awal tong sampah dengan offset dari posisi kamera
        initialPosition = new Vector3(cameraPosition.x, targetY, cameraPosition.z) + Vector3.forward * 2f;

        // Menyimpan posisi awal tong sampah
        transform.position = initialPosition;
    }

    void Update()
    {
        // Menghitung perpindahan tong sampah berdasarkan waktu
        float displacement = Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        // Mengupdate posisi tong sampah
        transform.position = initialPosition + Vector3.right * displacement;
    }

    public void IncreaseMoveSpeed()
    {
        moveSpeed += 0.5f; // Tambahkan kecepatan gerakan tong sampah
    }
}
