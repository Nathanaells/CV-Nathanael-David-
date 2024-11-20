using UnityEngine;
using System.Collections;

public class Metal : MonoBehaviour
{
    public int scoreValue = 10;
    public AudioClip trashSound;
    private AudioSource audioSource;
    private Renderer objectRenderer; // Menyimpan referensi ke komponen Renderer

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        objectRenderer = GetComponent<Renderer>(); // Mendapatkan komponen Renderer dari objek

        if (trashSound != null)
        {
            audioSource.clip = trashSound;
        }
        else
        {
            Debug.LogWarning("Audio clip untuk sound effect belum ditetapkan pada Metal script!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrashCan"))
        {
            ScoreManager.Instance.AddMetalScore(scoreValue);

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
                objectRenderer.enabled = false; // Matikan render object untuk membuatnya tidak terlihat
                StartCoroutine(DelayedDestroy());
            }
            else
            {
                Debug.LogWarning("Audio clip tidak ditetapkan pada Metal script atau AudioSource null!");
            }
        }
        else if (other.CompareTag("Lantai"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(audioSource.clip.length); // Tunggu sampai audio selesai diputar

        Destroy(gameObject);
    }
}
