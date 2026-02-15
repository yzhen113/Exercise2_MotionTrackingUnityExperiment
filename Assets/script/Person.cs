using UnityEngine;
using UnityEngine.SceneManagement;

public class Person : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip rewardSound;
    public AudioClip losingSound;
    public AudioClip winningSound;
    public float rewardSoundVolume = 1f;
    public float losingSoundVolume = 1f;
    public float winningSoundVolume = 1f;

    [Header("Game Settings")]
    public int totalGems = 4;

    private AudioSource audioSource;
    private int gemsCollected = 0;
    private bool gameEnded = false;

    /// <summary>Gems collected so far (for external display, e.g. ESP32 LCD).</summary>
    public int GemsCollected { get { return gemsCollected; } }
    /// <summary>True if player is still alive, false if game over.</summary>
    public bool IsAlive { get { return !gameEnded; } }
    private bool hasPlayedWinSound = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    // ===============================
    // COLLISION HANDLING FOR CHICKENS
    // ===============================
    void OnCollisionEnter(Collision collision)
    {
        if (gameEnded) return;

        // Chicken detection — only end game if hit from the front (within camera view)
        if (collision.gameObject.CompareTag("Chicken"))
        {
            if (IsChickenInFrontOfPlayer(collision.gameObject))
            {
                EndGameLoss();
            }
            // Hit from behind (outside camera view): do nothing, game continues
        }
    }

    /// <summary>
    /// Returns true if the chicken is in front of the player (within the first-person camera view).
    /// Hits from behind do not count as game over.
    /// </summary>
    private bool IsChickenInFrontOfPlayer(GameObject chicken)
    {
        Vector3 toChicken = (chicken.transform.position - transform.position).normalized;
        Camera cam = Camera.main;
        if (cam == null)
        {
            cam = GetComponentInChildren<Camera>();
        }
        if (cam == null)
        {
            // No camera found, use player forward as fallback
            return Vector3.Dot(transform.forward, toChicken) > 0f;
        }

        Vector3 playerForward = cam.transform.forward;
        // Dot > 0 means chicken is in front of the player (in camera view)
        return Vector3.Dot(playerForward, toChicken) > 0f;
    }

    // ===============================
    // TRIGGER HANDLING FOR GEMS
    // ===============================
    void OnTriggerEnter(Collider other)
    {
        if (gameEnded) return;

        if (other.CompareTag("Gem"))
        {
            CollectGem(other.gameObject);
        }
    }

    // ===============================
    // GEM COLLECTION
    // ===============================
    private void CollectGem(GameObject gem)
    {
        gem.SetActive(false);
        Destroy(gem);

        gemsCollected++;
        Debug.Log("Gems collected: " + gemsCollected);

        PlayRewardSound();

        if (gemsCollected >= totalGems && !hasPlayedWinSound)
        {
            hasPlayedWinSound = true;
            PlayWinningSound();
        }
    }

    // ===============================
    // GAME END (LOSS)
    // ===============================
    private void EndGameLoss()
    {
        if (gameEnded) return;

        gameEnded = true;

        Debug.Log("Player hit a chicken. Game Over.");

        PlayLosingSound();

        // Optional: freeze game
        Time.timeScale = 0f;
    }

    // ===============================
    // AUDIO
    // ===============================
    private void PlayRewardSound()
    {
        if (rewardSound != null)
            audioSource.PlayOneShot(rewardSound, rewardSoundVolume);
    }

    private void PlayWinningSound()
    {
        if (winningSound != null)
            audioSource.PlayOneShot(winningSound, winningSoundVolume);
    }

    private void PlayLosingSound()
    {
        if (losingSound != null)
            audioSource.PlayOneShot(losingSound, losingSoundVolume);
    }
}
