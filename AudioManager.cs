using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // ✅ Singleton
    public AudioSource engineSound; // ✅ Satu AudioSource global

    private void Awake()
    {
        // ✅ Pastikan hanya ada satu instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // ✅ Tetap hidup di setiap scene
    }
}
