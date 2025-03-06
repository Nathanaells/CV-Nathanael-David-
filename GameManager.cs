using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton agar bisa dipanggil dari mana saja
    public bool isGameCrashed = false; // ✅ Status global untuk kecelakaan

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ✅ Pastikan GameManager tidak dihapus saat ganti scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowVisualCommand(GameObject command, float displayTime)
    {
        StartCoroutine(ShowCommandCoroutine(command, displayTime));
    }

    IEnumerator ShowCommandCoroutine(GameObject command, float time)
    {
        if (command != null)
        {
            command.SetActive(true); // Tampilkan perintah
            yield return new WaitForSeconds(time); // Tunggu beberapa detik
            command.SetActive(false); // Sembunyikan perintah
        }
    }
}
