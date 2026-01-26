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

        // Chicken detection
        if (collision.gameObject.CompareTag("Chicken"))
        {
            EndGameLoss();
        }
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
