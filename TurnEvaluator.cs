using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEvaluator : MonoBehaviour
{
    public GameObject player;
    public GameObject incorrectTurnText;

    private PrometeoCarController carController;
    private TurnSignals turnSignals;
    private TrafficEvaluator trafficEvaluator;
    private Node turnSignalRoot;

    private Dictionary<string, bool> turnEvaluationStatus = new Dictionary<string, bool>();
    private bool isEvaluating = false; // ✅ Flag untuk mencegah evaluasi ganda

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            carController = player.GetComponent<PrometeoCarController>();
            turnSignals = player.GetComponent<TurnSignals>();
        }

        trafficEvaluator = FindObjectOfType<TrafficEvaluator>();

        if (trafficEvaluator == null)
        {
            Debug.LogError("TrafficEvaluator tidak ditemukan! Pastikan objek 'stop sign' aktif.");
        }

        if (turnSignals != null)
        {
            turnSignals.OnTurnSignalOff += HandleTurnSignalOff;
        }

        InitializeTurnEvaluationStatus();

        if (incorrectTurnText != null)
        {
            incorrectTurnText.SetActive(false);
        }

        turnSignalRoot = CreateTurnSignalBehaviorTree();
    }

    void Update()
    {
        if (carController == null || turnSignals == null) return;

   
        turnSignalRoot.Execute();
    }

    private Node CreateTurnSignalBehaviorTree()
    {
        // ✅ Evaluasi belokan (hanya jika sein menyala dan belum dievaluasi)
        Sequence evaluateTurnSequence = new Sequence();
        evaluateTurnSequence.AddChild(new CheckNode(() => !turnEvaluationStatus["TurnEvaluated"] && (turnSignals.IsLeftSignalOn() || turnSignals.IsRightSignalOn())));
        evaluateTurnSequence.AddChild(new ActionNode(() =>
        {
            if (!isEvaluating) // ✅ Pastikan evaluasi hanya berjalan sekali
            {
                if (turnSignals.IsLeftSignalOn())
                {
                    StartCoroutine(EvaluateTurnSignal(true));
                }
                else if (turnSignals.IsRightSignalOn())
                {
                    StartCoroutine(EvaluateTurnSignal(false));
                }
            }
        }));

        // ✅ Jika belokan benar
        Sequence correctTurnSequence = new Sequence();
        correctTurnSequence.AddChild(new CheckNode(() => turnEvaluationStatus["TurnSignalCorrect"]));
        correctTurnSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("✅ Belokan benar, menambah skor.");
            turnEvaluationStatus["TurnEvaluated"] = true; // ✅ Pastikan evaluasi dikunci

            if (trafficEvaluator != null)
            {
                trafficEvaluator.score += 10;
            }

            turnSignals.TurnOffSignals(); // ✅ Matikan lampu sein setelah belokan benar
        }));

        // ✅ Jika belokan salah, tampilkan pesan kesalahan
        Sequence incorrectTurnSequence = new Sequence();
        incorrectTurnSequence.AddChild(new CheckNode(() => turnEvaluationStatus["TurnSignalIncorrect"]));
        incorrectTurnSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("❌ Belokan salah, mengurangi skor.");
            turnEvaluationStatus["TurnEvaluated"] = true; // ✅ Pastikan evaluasi dikunci

            if (trafficEvaluator != null)
            {
                trafficEvaluator.score = Mathf.Max(0, trafficEvaluator.score - 10);
            }

            if (incorrectTurnText != null)
            {
                incorrectTurnText.SetActive(true);
                StartCoroutine(HideIncorrectTurnMessage());
            }

            turnEvaluationStatus["TurnSignalIncorrect"] = false; // ✅ Reset status setelah pesan kesalahan muncul
        }));

        // ✅ Node utama untuk menangani semua evaluasi
        ParallelNode root = new ParallelNode();
        root.AddChild(evaluateTurnSequence);
        root.AddChild(correctTurnSequence);
        root.AddChild(incorrectTurnSequence);

        return root;
    }

    IEnumerator EvaluateTurnSignal(bool isLeftSignal)
    {
        if (isEvaluating) yield break; // ✅ Mencegah evaluasi berjalan dua kali
        isEvaluating = true; // ✅ Tandai bahwa evaluasi sedang berjalan

    

        float timeLimit = 5f;
        float elapsedTime = 0f;
        turnEvaluationStatus["TurnSignalIncorrect"] = false;
        turnEvaluationStatus["TurnSignalCorrect"] = false;

        while (elapsedTime < timeLimit)
        {
            float steeringThreshold = 25f;
            bool isTurningLeft = carController.frontLeftCollider.steerAngle < -steeringThreshold;
            bool isTurningRight = carController.frontRightCollider.steerAngle > steeringThreshold;

            if ((isLeftSignal && isTurningLeft) || (!isLeftSignal && isTurningRight))
            {
                Debug.Log("✅ Belokan benar terdeteksi.");
                turnEvaluationStatus["TurnSignalCorrect"] = true;
                isEvaluating = false;
                yield break;
            }
            else if ((isLeftSignal && isTurningRight) || (!isLeftSignal && isTurningLeft))
            {
                Debug.Log("❌ Belokan salah terdeteksi.");
                turnEvaluationStatus["TurnSignalIncorrect"] = true;
                isEvaluating = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("⚠️ Sein dinyalakan tetapi tidak ada belokan dalam batas waktu!");
        isEvaluating = false;
    }

    IEnumerator HideIncorrectTurnMessage()
    {
        yield return new WaitForSeconds(3f);

        if (incorrectTurnText != null)
        {
            incorrectTurnText.SetActive(false);
        }
    }

    private void HandleTurnSignalOff()
    {
        InitializeTurnEvaluationStatus();
        isEvaluating = false; // ✅ Pastikan evaluasi bisa dilakukan lagi setelah sein mati
        Debug.Log("🔄 Lampu sein dimatikan, evaluasi di-reset!");
    }

    private void InitializeTurnEvaluationStatus()
    {
        turnEvaluationStatus["TurnSignalCorrect"] = false;
        turnEvaluationStatus["TurnSignalIncorrect"] = false;
        turnEvaluationStatus["TurnEvaluated"] = false;
        isEvaluating = false;
    }
}
