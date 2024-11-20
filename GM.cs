using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    public void Play()
    {
        // Memuat scene berdasarkan nama scene yang ingin dimuat
        SceneManager.LoadScene("Game");
    }

    // Metode Exit yang sama seperti sebelumnya
    public void Exit()
    {
         Application.Quit();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
