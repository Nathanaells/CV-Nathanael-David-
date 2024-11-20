using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject[] objectsToHide;

    void Start()
    {
        // Menyembunyikan panel pause dan objek-objek di belakangnya saat memulai permainan
        HidePanel();
    }

    // Fungsi untuk menampilkan panel pause
    public void Pause()
    {
        pauseMenuPanel.SetActive(true);

        // Menyembunyikan objek-objek di belakang panel
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(false);
        }

        Time.timeScale = 0f; // Jeda waktu dalam permainan
    }

    // Fungsi untuk melanjutkan permainan
    public void Resume()
    {
        pauseMenuPanel.SetActive(false);

        // Menampilkan kembali objek-objek yang disembunyikan
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(true);
        }

        Time.timeScale = 1f; // Mengaktifkan kembali waktu dalam permainan
    }

    // Fungsi untuk kembali ke menu utama
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Mengaktifkan kembali waktu dalam permainan sebelum keluar
        // Tambahkan kode untuk kembali ke menu utama
       SceneManager.LoadScene("MainMenu");
    }

    // Fungsi untuk menyembunyikan panel pause
    void HidePanel()
    {
        pauseMenuPanel.SetActive(false);
    }
}
