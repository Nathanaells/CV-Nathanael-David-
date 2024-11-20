using UnityEngine;
using UnityEngine.UI;

public class PauseMenu: MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button exitButton;

    private bool isPaused = false;

    void Start()
    {
        // Menyembunyikan panel pause saat memulai permainan
        pauseMenuUI.SetActive(false);

        // Menambahkan event listener untuk tombol resume
        resumeButton.onClick.AddListener(Resume);
        
        // Menambahkan event listener untuk tombol exit
        exitButton.onClick.AddListener(ExitToMainMenu);
    }

    void Update()
    {
        // Memeriksa jika permainan dijeda saat menekan tombol "Esc" pada keyboard
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // Fungsi untuk menampilkan panel pause
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f; // Menghentikan waktu dalam permainan
        pauseMenuUI.SetActive(true);
    }

    // Fungsi untuk melanjutkan permainan
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f; // Mengaktifkan kembali waktu dalam permainan
        pauseMenuUI.SetActive(false);
    }

    // Fungsi untuk kembali ke menu utama
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Mengaktifkan kembali waktu dalam permainan sebelum keluar
        // Menambahkan kode untuk kembali ke menu utama
        // Misalnya: SceneManager.LoadScene("MainMenu");
    }
}
