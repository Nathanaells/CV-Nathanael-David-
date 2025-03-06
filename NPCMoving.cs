using UnityEngine;

public class NPCMoving : MonoBehaviour
{
    public float speed = 5f;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float maxDistance = 10f;

    private Transform targetPoint;
    private AudioSource engineSound;
    private Rigidbody rb;
    private GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ✅ Ambil AudioSource dari AudioManager
        if (AudioManager.Instance != null)
        {
            engineSound = AudioManager.Instance.engineSound;
        }

        if (engineSound == null)
        {
            Debug.LogError("AudioSource tidak ditemukan di AudioManager!");
        }
    }

    void Update()
    {
        if (targetPoint == null || engineSound == null) return;

        // ✅ Gerakkan NPC ke target
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

        // ✅ Atur pitch berdasarkan kecepatan
        float normalizedSpeed = Mathf.InverseLerp(0, 10, speed);
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);

        // ✅ Atur volume suara berdasarkan jarak ke pemain
        AdjustEngineSound();

        // ✅ Hapus mobil saat sampai di titik B
        if (Vector3.Distance(transform.position, targetPoint.position) < 1f)
        {
            StopAndDestroy();
        }
    }

    private void AdjustEngineSound()
    {
        if (player == null || engineSound == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > maxDistance)
        {
            engineSound.volume = 0;
            if (engineSound.isPlaying)
                engineSound.Stop();
        }
        else
        {
            float newVolume = 0.3f - (distance / maxDistance);
            if (!engineSound.isPlaying)
                engineSound.Play();
            engineSound.volume = newVolume;
        }
    }

    private void StopAndDestroy()
    {
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        targetPoint = newTarget;
    }
}
