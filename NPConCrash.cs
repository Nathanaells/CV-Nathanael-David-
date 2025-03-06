using UnityEngine;
using UnityEngine.SceneManagement;

public class NPConCrash : MonoBehaviour
{
    public static bool isCrashed = false; // ✅ Cegah reset panel dari NPC lain
    private NPConCrash npcOnCrash;
    public GameObject crashPanel;  
    public GameObject retryCrashButton;


    public AudioSource audioSource; // Audio Source utama (suara tombol)
    public AudioClip buttonClickSound; // Suara klik tombol
    public AudioClip crashSound; // 🔊 Efek suara tabrakan

    private AudioSource crashAudioSource; // ✅ AudioSource khusus untuk suara tabrakan
    private PrometeoCarController carController; // Skrip kontrol mobil

    private void Awake()
    {
        isCrashed = false; // ✅ Reset setiap kali scene dimulai
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
    
    }

    private void Start()
{
    carController = FindObjectOfType<PrometeoCarController>();

    crashAudioSource = gameObject.AddComponent<AudioSource>();
    crashAudioSource.playOnAwake = false;
    crashAudioSource.loop = false;

    // ✅ Cek apakah crashPanel ditemukan, jika tidak, beri peringatan
    if (crashPanel == null)
    {
        crashPanel = GameObject.Find("Cinemachine/Canvas/CrashPanel");
        if (crashPanel == null)
        {
            Debug.LogError("🚨 [NPConCrash] CrashPanel tidak ditemukan di scene! Periksa namanya di Hierarchy.");
        }
    }

    if (retryCrashButton == null)
    {
        retryCrashButton = GameObject.Find("Cinemachine/Canvas/CrashPanel/Image/Retry");
        if (retryCrashButton == null)
        {
            Debug.LogError("🚨 [NPConCrash] RetryCrashButton tidak ditemukan di scene! Periksa namanya di Hierarchy.");
        }
    }

    // ✅ Jika crashPanel ditemukan, sembunyikan
    if (crashPanel != null) crashPanel.SetActive(false);
    if (retryCrashButton != null) retryCrashButton.SetActive(false);
}
    
    private void OnTriggerEnter(Collider other)
{
    if (!isCrashed && other.CompareTag("Player") && !GameManager.instance.isGameCrashed)
    {
        isCrashed = true;
        GameManager.instance.isGameCrashed = true;
        PlayCrashSound();
        DisableAllSoundsExceptCrashAndButtons();
        DisableCarControl();

        HideActiveVisualCommands();
        ShowCrashScreen();
        
        // 🔴 Matikan NPCSpawner agar tidak spawn NPC baru
        NPCSpawner npcSpawner = FindObjectOfType<NPCSpawner>();
        if (npcSpawner != null)
        {
            npcSpawner.StopSpawning();
        }

        Invoke("FreezeTime", 2f);
    }
}


    void ShowCrashScreen()
    {
        if (crashPanel != null)
        {
            crashPanel.SetActive(true);
        }

        if (retryCrashButton != null)
        {
            retryCrashButton.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        PlayButtonSound();
        Debug.Log("🔄 Restart Level: Reset isCrashed ke false");
        
        isCrashed = false; // ✅ Reset status kecelakaan
        
        // ✅ Pastikan skrip PrometeoCarController tetap ada
        if (carController == null)
        {
            carController = FindObjectOfType<PrometeoCarController>();
            if (carController == false)
            {
                Debug.Log("✅ Skrip PrometeoCarController ditambahkan kembali!");
            }
        }
        else
        {
            carController.enabled = true; // ✅ Aktifkan kembali skrip setelah restart
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DisableCarControl()
    {
        if (carController != null)
        {
            carController.enabled = false; // ✅ Matikan skrip tanpa menghapusnya
        }
    }

    private void EnableCarControl()
    {
        if (carController != null)
        {
            carController.enabled = true; // ✅ Hidupkan kembali setelah restart
        }
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void PlayCrashSound()
    {
        if (crashSound != null)
        {
            crashAudioSource.clip = crashSound;
            crashAudioSource.Play(); // ✅ Mainkan suara tabrakan
        }
    }

    private void DisableAllSoundsExceptCrashAndButtons()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource && source != crashAudioSource) // ✅ Jangan matikan tombol & suara tabrakan
            {
                source.Stop();
            }
        }
    }

    private void HideActiveVisualCommands()
    {
        WaypointTrigger[] waypointScripts = FindObjectsOfType<WaypointTrigger>(); // ✅ Temukan semua skrip WaypointTrigger

        foreach (WaypointTrigger waypoint in waypointScripts)
        {
            if (waypoint.visualCommand != null) // ✅ Pastikan ada perintah visual
            {
                Destroy(waypoint.visualCommand);
            }
          
        }
    }

    private void FreezeTime()
    {
        Time.timeScale = 0f; // 🔴 Bekukan waktu setelah 2 detik
    }
}
