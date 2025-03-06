using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Tambahkan namespace UI

public class TurnSignals : MonoBehaviour
{
    public Light leftSignalLight;
    public Light rightSignalLight;
    
    public Button leftSignalButton;  // Tambahkan tombol UI sein kiri
    public Button rightSignalButton; // Tambahkan tombol UI sein kanan

    private bool isLeftSignalOn = false;
    private bool isRightSignalOn = false;
    private Coroutine leftSignalCoroutine;
    private Coroutine rightSignalCoroutine;

    // Event untuk memberi tahu ketika sein dimatikan
    public event Action OnTurnSignalOff;

    void Start()
    {
        // Tambahkan listener untuk button UI
        if (leftSignalButton != null)
            leftSignalButton.onClick.AddListener(ToggleLeftSignal);
        
        if (rightSignalButton != null)
            rightSignalButton.onClick.AddListener(ToggleRightSignal);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleLeftSignal();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleRightSignal();
        }
    }

    void ToggleLeftSignal()
    {
        if (isLeftSignalOn)
        {
            StopSignal(ref leftSignalCoroutine, leftSignalLight);
            isLeftSignalOn = false;
            OnTurnSignalOff?.Invoke(); // Panggil event saat sein dimatikan
        }
        else
        {
            if (isRightSignalOn) ToggleRightSignal();
            isLeftSignalOn = true;
            leftSignalCoroutine = StartCoroutine(BlinkSignal(leftSignalLight));
        }
    }

    void ToggleRightSignal()
    {
        if (isRightSignalOn)
        {
            StopSignal(ref rightSignalCoroutine, rightSignalLight);
            isRightSignalOn = false;
            OnTurnSignalOff?.Invoke(); // Panggil event saat sein dimatikan
        }
        else
        {
            if (isLeftSignalOn) ToggleLeftSignal();
            isRightSignalOn = true;
            rightSignalCoroutine = StartCoroutine(BlinkSignal(rightSignalLight));
        }
    }

    public void TurnOffSignals()
{
    StopSignal(ref leftSignalCoroutine, leftSignalLight);
    StopSignal(ref rightSignalCoroutine, rightSignalLight);
    
    isLeftSignalOn = false;
    isRightSignalOn = false;
    
    OnTurnSignalOff?.Invoke(); // Memicu event bahwa sein telah mati
}


    IEnumerator BlinkSignal(Light signalLight)
    {
        while (true)
        {
            signalLight.enabled = !signalLight.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void StopSignal(ref Coroutine signalCoroutine, Light signalLight)
    {
        if (signalCoroutine != null)
        {
            StopCoroutine(signalCoroutine);
            signalCoroutine = null;
        }
        signalLight.enabled = false;
    }

    // Getter untuk mengecek status sein
    public bool IsLeftSignalOn() => isLeftSignalOn;
    public bool IsRightSignalOn() => isRightSignalOn;
}
