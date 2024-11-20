using UnityEngine;
using UnityEngine.UI;

public class BestScore: MonoBehaviour
{
    public Text bestScoreText;

    private void Start()
    {
        // Ambil skor tertinggi dari PlayerPrefs
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        // Tampilkan skor tertinggi di UI
        bestScoreText.text = "Best Score: " + bestScore.ToString();
    }
}
