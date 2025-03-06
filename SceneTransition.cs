using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    public GameObject transitionObject; // GameObject yang menyimpan Animator
    public Animator transitionAnimator;
    public float transitionTime = 1f;
    public AudioSource sfx;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (transitionObject != null)
        {
            transitionObject.SetActive(false); // Pastikan animasi dalam keadaan nonaktif di awal
        }
    }

    // 🔹 Fungsi untuk mengganti scene dengan animasi Fade In
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        if (transitionObject == null || transitionAnimator == null || transitionAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError("Transition Object atau Animator tidak valid! Cek Inspector.");
            yield break;
        }

        // Aktifkan transitionObject sebelum memainkan animasi
        if (!transitionObject.activeSelf)
        {
            transitionObject.SetActive(true);
        }

        // Mainkan SFX jika ada
        if (sfx != null) sfx.Play();

        // Jalankan animasi FadeIn
        transitionAnimator.SetTrigger("FadeIn");

        // Tunggu hingga animasi selesai
        yield return new WaitForSeconds(transitionTime);

        // Load scene baru
        SceneManager.LoadScene(sceneName);
    }

    // 🔹 Fungsi untuk mengganti scene tanpa animasi tetapi tetap memainkan SFX
    public void ChangeSceneInstant(string sceneName)
    {
        StartCoroutine(LoadSceneInstant(sceneName));
    }

    IEnumerator LoadSceneInstant(string sceneName)
    {
        // Mainkan SFX jika ada
        if (sfx != null) sfx.Play();

        // Tunggu sebentar agar SFX terdengar sebelum scene berubah (opsional)
        yield return new WaitForSeconds(0.3f); 

        // Load scene langsung tanpa animasi
        SceneManager.LoadScene(sceneName);
    }

    // 🔹 Fungsi untuk berpindah ke halaman tutorial
    public void GoToTutorialPage()
    {
        PlayButtonSound();
        ChangeScene("TutorialScene"); // Ganti nama scene sesuai dengan yang ada di Build Settings
    }

    // 🔹 Fungsi untuk keluar dari permainan
    public void ExitGame()
    {
        PlayButtonSound();
        StartCoroutine(ExitWithDelay());
    }

    IEnumerator ExitWithDelay()
    {
        if (transitionObject != null && transitionAnimator != null)
        {
            transitionObject.SetActive(true);
            transitionAnimator.SetTrigger("FadeIn");

            if (sfx != null) sfx.Play();
            yield return new WaitForSeconds(transitionTime);
        }

        Application.Quit();
    }

    // 🔹 Fungsi untuk memainkan suara tombol saat ditekan
    private void PlayButtonSound()
    {
        if (sfx != null) sfx.Play();
    }
}
