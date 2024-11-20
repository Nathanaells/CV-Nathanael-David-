using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public Text scoreText;
    private int score = 0;
    private TrashCanMovement trashCanMovement; // Referensi ke kelas TrashCanMovement

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // Ambil referensi ke TrashCanMovement saat memulai
        trashCanMovement = FindObjectOfType<TrashCanMovement>();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
        UpdateBestScore(); // Panggil fungsi untuk memperbarui skor tertinggi
        CheckScoreForSpeedIncrease(); // Periksa skor untuk peningkatan kecepatan
    }

    public void AddMetalScore(int value)
    {
        // Tambahkan skor logam dan skor total
        score += value;
        UpdateScoreText();
        UpdateBestScore();
        CheckScoreForSpeedIncrease(); // Periksa skor untuk peningkatan kecepatan
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void UpdateBestScore()
    {
        // Perbarui skor tertinggi jika skor saat ini melebihi skor tertinggi yang tersimpan
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
            PlayerPrefs.Save();
        }
    }

    private void CheckScoreForSpeedIncrease()
    {
        // Jika skor adalah kelipatan 50, tambahkan kecepatan gerakan tong sampah
        if (score % 50 == 0 && score != 0 && trashCanMovement != null)
        {
            trashCanMovement.IncreaseMoveSpeed();
        }
    }
}
