using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioClip sfxClip; // Masukkan audio clip untuk efek suara di Unity Editor
    private AudioSource sfxSource;

    void Start()
    {
        // Pastikan objek ini tidak dihancurkan saat berpindah scene
        DontDestroyOnLoad(gameObject);

 
    }

}
