using System.Collections;
using UnityEngine;

public class WaypointTrigger : MonoBehaviour
{
    [Header("Perintah yang Akan Ditampilkan")]
    public GameObject visualCommand; // Objek perintah yang akan muncul
    public float displayTime = 3f;   // Waktu tampil sebelum menghilang

    private static WaypointTrigger activeWaypoint = null; // Simpan waypoint yang sedang aktif
    private Coroutine hideCoroutine = null; // Simpan coroutine yang sedang berjalan
    private MeshRenderer meshRenderer; // Untuk menyembunyikan waypoint

    private void Start()
    {
        InitializeWaypoint();
    }

    private void InitializeWaypoint()
    {
        // Sembunyikan visualCommand di awal
        if (visualCommand != null)
        {
            visualCommand.SetActive(false);
        }

        // Ambil MeshRenderer Waypoint
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("❌ MeshRenderer tidak ditemukan pada " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Matikan waypoint aktif sebelumnya jika ada
            if (activeWaypoint != null && activeWaypoint != this)
            {
                activeWaypoint.DeactivateWaypoint();
            }

            // Aktifkan waypoint baru ini
            ActivateWaypoint();
        }
    }

    private void ActivateWaypoint()
    {
        activeWaypoint = this; // Simpan waypoint ini sebagai yang aktif

        // Sembunyikan waypoint dengan mematikan MeshRenderer
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // Aktifkan perintah visual
        if (visualCommand != null)
        {
            visualCommand.SetActive(true);

            // Hentikan coroutine sebelumnya jika masih berjalan
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }

            // Mulai coroutine untuk menyembunyikan perintah setelah waktu tertentu
            hideCoroutine = StartCoroutine(HideVisualCommandAfterTime());
        }
    }

    private void DeactivateWaypoint()
    {
        // Sembunyikan perintah yang sedang aktif
        if (visualCommand != null)
        {
            visualCommand.SetActive(false);
        }

        // Hentikan coroutine yang sedang berjalan
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private IEnumerator HideVisualCommandAfterTime()
    {
        yield return new WaitForSeconds(displayTime);

        // Sembunyikan perintah jika masih aktif
        if (visualCommand != null)
        {
            visualCommand.SetActive(false);
        }

        // Jika ini adalah waypoint aktif, kosongkan referensinya
        if (activeWaypoint == this)
        {
            activeWaypoint = null;
        }

        Destroy(gameObject); // Hancurkan waypoint setelah selesai
    }
}
