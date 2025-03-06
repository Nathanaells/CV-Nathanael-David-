using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightEvaluator : MonoBehaviour
{
    public GameObject player;
    private PrometeoCarController carController;
    private TrafficEvaluator trafficEvaluator;
    private Node rootNode;
    private bool isAtRedLight = false;

    private bool hasScoredStop = false;  
    private bool hasScoredViolation = false;  

    public TrafficLight trafficLight;

    [Header("NPC Hukuman")]
    public GameObject npcCarPrefab; // Prefab mobil NPC
    public Transform npcSpawnPoint; // Titik A (tempat NPC muncul)
    public Transform npcTargetPoint; // Titik B (tujuan NPC)
    public float npcSpeed = 20f; // Kecepatan mobil NPC
    private GameObject spawnedNpcCar; // Referensi ke mobil NPC yang dibuat

    void Start()
    {
        if (player != null)
        {
            carController = player.GetComponent<PrometeoCarController>();
            trafficEvaluator = FindObjectOfType<TrafficEvaluator>();
        }

        if (trafficEvaluator == null)
        {
            Debug.LogError("❌ TrafficEvaluator tidak ditemukan!");
        }

        if (trafficLight == null)
        {
            trafficLight = FindObjectOfType<TrafficLight>();
            if (trafficLight == null)
            {
                Debug.LogError("❌ TrafficLight tidak ditemukan!");
            }
        }

        rootNode = CreateBehaviourTree();
    }

    void Update()
    {
        if (isAtRedLight && rootNode != null)
        {
            rootNode.Execute();
        }

        if (spawnedNpcCar != null)
        {
            MoveNpcCar(); // Gerakkan NPC jika sudah muncul
        }
    }

    private Node CreateBehaviourTree()
    {
        // **Node Berhenti di Lampu Merah (Hanya Sekali)**
        Sequence stopAtRedLightSequence = new Sequence();
        stopAtRedLightSequence.AddChild(new CheckNode(() => isAtRedLight && !hasScoredStop));
        stopAtRedLightSequence.AddChild(new CheckNode(() => carController.carSpeed < 5));
        stopAtRedLightSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("✅ Pemain berhenti di lampu merah, skor +10");
            if (trafficEvaluator != null) trafficEvaluator.score += 10;
            hasScoredStop = true;
        }));

        // **Node Melanggar Lampu Merah & Memunculkan NPC (Hanya Sekali)**
        Sequence runRedLightSequence = new Sequence();
        runRedLightSequence.AddChild(new CheckNode(() => isAtRedLight && !trafficLight.IsGreenLightOn && !hasScoredViolation));
        runRedLightSequence.AddChild(new CheckNode(() => carController.carSpeed > 20));
        runRedLightSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("❌ Pemain menerobos lampu merah, skor -20");
            if (trafficEvaluator != null) trafficEvaluator.score = Mathf.Max(0, trafficEvaluator.score - 20);
            hasScoredViolation = true;

            // Memunculkan NPC yang bergerak dari A ke B
            SpawnNpcCar();
        }));

        ParallelNode parallelNode = new ParallelNode();
        parallelNode.AddChild(stopAtRedLightSequence);
        parallelNode.AddChild(runRedLightSequence);

        return parallelNode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAtRedLight = trafficLight != null && trafficLight.IsRedLightOn;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚦 Pemain keluar dari area lampu lalu lintas.");
            isAtRedLight = false;

            hasScoredStop = false;
            hasScoredViolation = false;
        }
    }

    // **Memunculkan NPC Mobil**
    private void SpawnNpcCar()
    {
        if (npcCarPrefab != null && npcSpawnPoint != null && npcTargetPoint != null)
        {
            spawnedNpcCar = Instantiate(npcCarPrefab, npcSpawnPoint.position, npcSpawnPoint.rotation);
            spawnedNpcCar.transform.LookAt(npcTargetPoint); // ✅ Otomatis menghadap target
            Debug.Log("🚗 NPC Mobil muncul dan mulai berjalan!");
        }
        else
        {
            Debug.LogError("❌ Tidak dapat memunculkan NPC mobil! Pastikan prefab dan titik spawn sudah diatur.");
        }
    }

    // **Menggerakkan NPC dari Titik A ke Titik B**
    private void MoveNpcCar()
    {
        if (spawnedNpcCar != null)
        {
            spawnedNpcCar.transform.position = Vector3.MoveTowards(
                spawnedNpcCar.transform.position,
                npcTargetPoint.position,
                npcSpeed * Time.deltaTime
            );

            // **Jika sudah mencapai Titik B, NPC dihancurkan**
            if (Vector3.Distance(spawnedNpcCar.transform.position, npcTargetPoint.position) < 0.5f)
            {
                Destroy(spawnedNpcCar);
                Debug.Log("🛑 NPC Mobil telah mencapai tujuan dan dihancurkan.");
            }
        }
    }
}
