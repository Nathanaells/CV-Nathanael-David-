using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAudio : MonoBehaviour
{
    // AudioSource untuk memutar suara.
    public AudioSource audioSource;

    // Referensi ke GameObject Player.
    public GameObject player;

    // Jarak maksimum agar suara masih terdengar.
    public float maxDistance = 10f;

    void Start()
    {
        // Jika audioSource belum di-assign, ambil komponen AudioSource dari GameObject ini.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Hitung jarak antara NPC dan Player.
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // Jika dalam jarak maxDistance, atur volume berdasarkan kedekatan.
            if (distance <= maxDistance)
            {
                float volume = 0.3f - (distance / maxDistance);
                audioSource.volume = volume;

                // Jika audio belum dimainkan, mainkan.
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                // Jika di luar jarak maxDistance, hentikan audio.
                audioSource.volume = 0;
            }
        }
    }
}
