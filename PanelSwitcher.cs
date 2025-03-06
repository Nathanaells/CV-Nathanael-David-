using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject panel1;  // Panel pertama
    public GameObject panel2;  // Panel kedua
    public string mainMenuScene;  // Nama scene untuk kembali ke main menu
    public AudioSource buttonSFX; // Suara saat tombol ditekan

    private void Start()
    {
        // Pastikan hanya satu panel yang aktif di awal
        if (panel1 != null && panel2 != null)
        {
            panel1.SetActive(true);
            panel2.SetActive(false);
        }
    }

    // 🔹 Fungsi untuk berpindah antar panel (Previous dan Next)
    public void SwitchPanel()
    {
        if (buttonSFX != null) buttonSFX.Play(); // Mainkan suara tombol jika ada

        if (panel1 != null && panel2 != null)
        {
            bool isPanel1Active = panel1.activeSelf;

            panel1.SetActive(!isPanel1Active);
            panel2.SetActive(isPanel1Active);
        }
    }

    // 🔹 Fungsi untuk kembali ke Main Menu
    public void GoToMainMenu()
    {
        if (buttonSFX != null) buttonSFX.Play(); // Mainkan suara tombol jika ada
        SceneManager.LoadScene(mainMenuScene);
    }
}
