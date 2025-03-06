using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    public GameObject finishPanel; // Panel UI saat finish
    public Text scoreText; // UI untuk menampilkan skor
    public string nextSceneName; // Nama scene berikutnya
    public GameObject Restart;

    public GameObject retryButton; // Tombol "Restart"
    public GameObject nextButton; // Tombol "Lanjut"
    public GameObject replayButton; // Tombol "Ulangi"
    public GameObject WinText;

    public AudioSource audioSource; // Audio Source untuk suara tombol
    public AudioClip buttonClickSound; // Suara klik tombol

    private bool gameFinished = false;
    private int minScoreToPass = 30; // Minimal skor agar bisa lanjut

    private PrometeoCarController carController; // Skrip kontrol mobil

    private void Start()
    {
        carController = FindObjectOfType<PrometeoCarController>(); // Cari skrip kontrol mobil
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameFinished && other.CompareTag("Player")) // Pastikan hanya berjalan sekali
        {
            gameFinished = true;
            DisableCarControl(); // Nonaktifkan kontrol mobil
            ShowFinishScreen();
        }
    }

    void ShowFinishScreen()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(true); // Tampilkan panel hasil
        }

        TrafficEvaluator trafficEvaluator = FindObjectOfType<TrafficEvaluator>();
        if (trafficEvaluator != null && scoreText != null)
        {
            scoreText.text = "Score: " + trafficEvaluator.score; // Tampilkan skor
        }

        // Menampilkan tombol berdasarkan skor
        if (trafficEvaluator != null && trafficEvaluator.score >= minScoreToPass)
        {
            nextButton.SetActive(true);  // Tampilkan tombol "Lanjut"
            replayButton.SetActive(true); // Tampilkan tombol "Ulangi"
            retryButton.SetActive(false); // Sembunyikan tombol "Restart"
            WinText.SetActive(true);
            Restart.SetActive(false);
        }
        else
        {
            Restart.SetActive(true);
            WinText.SetActive(false);
            retryButton.SetActive(true);  // Tampilkan tombol "Restart"
            nextButton.SetActive(false);  // Sembunyikan tombol "Lanjut"
            replayButton.SetActive(false); // Sembunyikan tombol "Ulangi"
        }
    }

    public void LoadNextLevel()
    {
        PlayButtonSound();
        EnableCarControl(); // Aktifkan kembali kontrol mobil
        SceneManager.LoadScene(nextSceneName);
    }

    public void RestartLevel()
    {
        PlayButtonSound();
        EnableCarControl(); // Aktifkan kembali kontrol mobil
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DisableCarControl()
    {
        if (carController != null)
        {
            carController.enabled = false; // Nonaktifkan skrip kontrol mobil
        }
    }

    private void EnableCarControl()
    {
        if (carController != null)
        {
            carController.enabled = true; // Aktifkan kembali skrip kontrol mobil
        }
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}
