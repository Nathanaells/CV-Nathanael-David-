using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficEvaluator : MonoBehaviour
{
    public GameObject player; // Referensi ke objek pemain
    public float speedLimit = 20f; // Batas kecepatan maksimum
    public float speedLimit2 = 20f;
    public int score = 0; // Skor pemain
    private Node rootNode;
    private TurnSignals turnSignals; 
    private bool isInEvaluationZone = false;
    private PrometeoCarController carController;
    private bool turnSignalValid = false; 
    private bool turnSignalMismatch = false; // Menandai apakah belokan salah


    // Status evaluasi untuk memastikan evaluasi hanya terjadi sekali
    private Dictionary<string, bool> evaluationStatus = new Dictionary<string, bool>();

    void Start()
{
    if (player != null)
    {
        carController = player.GetComponent<PrometeoCarController>();
        turnSignals = player.GetComponent<TurnSignals>();

        // Daftarkan event agar tahu kapan sein dimatikan
        if (turnSignals != null)
        {
            turnSignals.OnTurnSignalOff += HandleTurnSignalOff;
        }
    }

    InitializeEvaluationStatus();
    rootNode = CreateBehaviourTree();
}

   void Update()
{
    if (isInEvaluationZone && rootNode != null)
    {
        rootNode.Execute();
    }

    if (carController == null || turnSignals == null) return;

    bool leftSignalOn = turnSignals.IsLeftSignalOn(); 
    bool rightSignalOn = turnSignals.IsRightSignalOn(); 
    
    // Reset turnSignalValid jika lampu sein mati
    if (!leftSignalOn && !rightSignalOn)
    {
        turnSignalValid = false;
    }

    // Jika pemain menyalakan sein, mulai evaluasi dengan coroutine
    if (leftSignalOn)
    {
        StopAllCoroutines(); 
        StartCoroutine(EvaluateTurnSignal(true));
    }
    else if (rightSignalOn)
    {
        StopAllCoroutines();
        StartCoroutine(EvaluateTurnSignal(false));
    }
}

    private void InitializeEvaluationStatus()
    {
        evaluationStatus["SpeedBelowLimit"] = false;
        evaluationStatus["SpeedBeyondLimit"] = false;
        evaluationStatus["StoppedLongEnough"] = false;
        evaluationStatus["RedLightObeyed"] = false;
        evaluationStatus["TurnSignalCorrect"] = false;
        evaluationStatus["TurnSignalIncorrect"] = false;
    }

    private Node CreateBehaviourTree()
    {
        // Evaluasi Kecepatan di Bawah Batas
        Sequence speedBelowLimitSequence = new Sequence();
        speedBelowLimitSequence.AddChild(new CheckNode(() => !evaluationStatus["SpeedBelowLimit"]));
        speedBelowLimitSequence.AddChild(new CheckNode(() => IsSpeedBelowLimit(speedLimit)));
        speedBelowLimitSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("Kecepatan Aman.");
            score += 10;
            evaluationStatus["SpeedBelowLimit"] = true;
        }));

        // Evaluasi Kecepatan Melebihi Batas
        Sequence speedBeyondLimitSequence = new Sequence();
        speedBeyondLimitSequence.AddChild(new CheckNode(() => !evaluationStatus["SpeedBeyondLimit"]));
        speedBeyondLimitSequence.AddChild(new CheckNode(() => IsSpeedBeyondLimit(speedLimit2)));
        speedBeyondLimitSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("Kecepatan tidak aman.");
            score = Mathf.Max(0, score - 10);
            evaluationStatus["SpeedBeyondLimit"] = true;
        }));



        // Evaluasi Lampu Sein
        Sequence turnSignalSequence = new Sequence();
        turnSignalSequence.AddChild(new CheckNode(() => !evaluationStatus["TurnSignalCorrect"]));
         turnSignalSequence.AddChild(new CheckNode(() => turnSignalValid)); // Mengecek apakah sein benar
        turnSignalSequence.AddChild(new ActionNode(() =>
        {
           Debug.Log("Penggunaan lampu sein benar.");
           score += 10;
           evaluationStatus["TurnSignalCorrect"] = true;
        }));

        Sequence incorrectTurnSequence = new Sequence();
        incorrectTurnSequence.AddChild(new CheckNode(() => !evaluationStatus["TurnSignalIncorrect"]));
        incorrectTurnSequence.AddChild(new CheckNode(() => turnSignalMismatch)); // Hanya jika belokan salah
        incorrectTurnSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("Sein ke arah yang salah! Skor dikurangi 10.");
            score = Mathf.Max(0, score - 10);
            evaluationStatus["TurnSignalIncorrect"] = true;
        }));


        // Parallel Node: Evaluasi Semua Aturan
        ParallelNode parallelNode = new ParallelNode();
        parallelNode.AddChild(speedBelowLimitSequence);
        parallelNode.AddChild(speedBeyondLimitSequence);
        parallelNode.AddChild(incorrectTurnSequence);
        parallelNode.AddChild(turnSignalSequence);

        return parallelNode;
    }

    private bool IsSpeedBelowLimit(float speedLimit)
    {
        return carController != null && carController.carSpeed > 5 && carController.carSpeed <= speedLimit;
    }

    private bool IsSpeedBeyondLimit(float speedLimit2)
    {
        return carController != null && carController.carSpeed >= speedLimit2;
    }


IEnumerator EvaluateTurnSignal(bool isLeftSignal)
{
    float timeLimit = 5f; // Waktu maksimal untuk melakukan belokan setelah sein dinyalakan
    float elapsedTime = 0f;
    turnSignalMismatch = false;

    while (elapsedTime < timeLimit)
    {
        float steeringThreshold = 5f;
        bool isTurningLeft = carController.frontLeftCollider.steerAngle < -steeringThreshold;
        bool isTurningRight = carController.frontRightCollider.steerAngle > steeringThreshold;

        if ((isLeftSignal && isTurningLeft) || (!isLeftSignal && isTurningRight))
        {
            turnSignalValid = true;
            yield break; 
        }
        else if((isLeftSignal && isTurningRight) || (!isLeftSignal && isTurningLeft)){
            turnSignalMismatch = true;
            yield break;
        }
      
        elapsedTime += Time.deltaTime;
        yield return null; 
    }

    turnSignalValid = false;
    Debug.Log("Sein dinyalakan tetapi tidak ada belokan dalam batas waktu!");
}

    
    private bool IsTurnSignalCorrect()
    {
        return turnSignalValid; 
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pemain memasuki area evaluasi.");
            isInEvaluationZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pemain keluar dari area evaluasi.");
            isInEvaluationZone = false;
            InitializeEvaluationStatus();
        }
    }

    private void HandleTurnSignalOff()
{
    turnSignalValid = false; // Reset status evaluasi
    StopAllCoroutines(); 
    Debug.Log("Lampu sein dimatikan, turnSignalValid di-reset!");
}

public void DecreaseScore(int amount)
{
    score = Mathf.Max(0, score - amount); // Kurangi skor, tetapi tidak boleh negatif
    Debug.Log("Skor saat ini: " + score);
}

}