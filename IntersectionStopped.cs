using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionStopped : MonoBehaviour
{
    public GameObject player; // Referensi ke objek pemain
    private PrometeoCarController carController;
    private TrafficEvaluator trafficEvaluator; // Referensi ke TrafficEvaluator
    private Node rootNode;
    private bool isInIntersection = false;
    private bool hasStopped = false;
    private bool hasEvaluatedExit = false;

    void Start()
    {
        if (player == null)
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    if (player != null)
    {
        carController = player.GetComponent<PrometeoCarController>();
        trafficEvaluator = player.GetComponent<TrafficEvaluator>();
    }

    if (trafficEvaluator == null)
    {
        trafficEvaluator = FindObjectOfType<TrafficEvaluator>();
    }

        rootNode = CreateBehaviourTree();
    }

    void Update()
    {
        if (isInIntersection && rootNode != null)
        {
            rootNode.Execute();
        }
    }

    private Node CreateBehaviourTree()
    {
        // Evaluasi berhenti di perempatan
        Sequence stopAtIntersectionSequence = new Sequence();
        stopAtIntersectionSequence.AddChild(new CheckNode(() => !hasStopped));
        stopAtIntersectionSequence.AddChild(new CheckNode(() => carController.carSpeed < 3));
        stopAtIntersectionSequence.AddChild(new ActionNode(() =>
        {
            Debug.Log("Pemain berhenti di perempatan, skor +10");
            if (trafficEvaluator != null) trafficEvaluator.score += 10;
            hasStopped = true;
        }));

        // Evaluasi kecepatan saat keluar dari perempatan
        Sequence exitIntersectionSequence = new Sequence();
        exitIntersectionSequence.AddChild(new CheckNode(() => !hasEvaluatedExit));
        exitIntersectionSequence.AddChild(new ActionNode(() =>
        {
            float speed = carController.carSpeed;
            if (speed > 20f)
            {
                Debug.Log("Pemain melewati perempatan dengan kecepatan tinggi, skor -10");
                if (trafficEvaluator != null) trafficEvaluator.score = Mathf.Max(0, trafficEvaluator.score - 10);
            }
            else
            {
                Debug.Log("Pemain melewati perempatan dengan kecepatan aman, skor +10");
                if (trafficEvaluator != null) trafficEvaluator.score += 10;
            }
            hasEvaluatedExit = true;
        }));

        // Parallel Node untuk menjalankan evaluasi secara bersamaan
        ParallelNode parallelNode = new ParallelNode();
        parallelNode.AddChild(stopAtIntersectionSequence);
        parallelNode.AddChild(exitIntersectionSequence);

        return parallelNode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pemain memasuki area perempatan.");
            isInIntersection = true;
            hasEvaluatedExit = false; // Reset evaluasi keluar saat masuk kembali
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pemain keluar dari area perempatan.");
            isInIntersection = false;
            hasStopped = false; // Reset status evaluasi
        }
    }
}
