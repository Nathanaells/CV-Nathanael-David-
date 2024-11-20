using UnityEngine;

public class TrashThrower : MonoBehaviour
{
    public GameObject basketballPrefab; // Prefab bola basket
    public float throwForce = 10f;

    private Vector2 swipeStartPos; // Posisi awal swipe
    private bool isSwiping = false; // Menandakan apakah sedang melakukan swipe

    private Vector3 throwerPosition; // Posisi awal pelempar

    void Start()
    {
        // Menetapkan posisi awal pelempar sesuai keinginan
        SetThrowerPosition(new Vector3(0f, 0.5f, 0f)); // Contoh: Posisi (0, 1, -1.5)
    }

    void Update()
    {
        // Menangani input swipe pada layar sentuh
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0]; // Mendapatkan informasi touch pertama

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    swipeStartPos = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (isSwiping)
                    {
                        Vector2 swipeDirection = touch.position - swipeStartPos;
                        ThrowBasketball(swipeDirection.normalized);
                        isSwiping = false;
                    }
                    break;
            }
        }
    }

    // Fungsi untuk mengatur posisi awal pelempar
    public void SetThrowerPosition(Vector3 newPosition)
    {
        throwerPosition = newPosition;
    }

    void ThrowBasketball(Vector2 direction)
    {
        // Menginstantiate prefab bola basket di posisi yang telah ditetapkan untuk pelempar
        GameObject basketball = Instantiate(basketballPrefab, throwerPosition, Quaternion.identity);

        // Melakukan lemparan bola basket dari posisi pelempar berdasarkan arah swipe
        Rigidbody rb = basketball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(new Vector3(direction.x, 0f, direction.y) * throwForce, ForceMode.Impulse);
        }
    }
}
