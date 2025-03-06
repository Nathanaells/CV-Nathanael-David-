using UnityEngine;

public class WrongLaneScript: MonoBehaviour
{
    public int penaltyScore = 10; // Skor yang dikurangi saat mengenai rintangan
    private TrafficEvaluator trafficEvaluator; // Referensi ke TrafficEvaluator

    void Start()
    {
        trafficEvaluator = FindObjectOfType<TrafficEvaluator>();
        if (trafficEvaluator == null)
        {
            Debug.LogError("TrafficEvaluator tidak ditemukan! Pastikan ada di scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && trafficEvaluator != null) // Pastikan hanya pemain yang memicu rintangan
        {
            trafficEvaluator.DecreaseScore(penaltyScore);
            Debug.Log("Pemain melanggar jalur!");
        }
    }
}