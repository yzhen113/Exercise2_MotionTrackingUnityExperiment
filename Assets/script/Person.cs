using UnityEngine;
using UnityEngine.SceneManagement;

public class Person : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip rewardSound;
    public AudioClip losingSound;
    public float rewardSoundVolume = 1f;
    public float losingSoundVolume = 1f;
    
    private AudioSource audioSource;
    private bool gameEnded = false;
    
    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = rewardSoundVolume;
        }
        
        // Try to load reward sound if not assigned (from Resources folder)
        if (rewardSound == null)
        {
            rewardSound = Resources.Load<AudioClip>("reward");
        }
        
        // Try to load losing sound if not assigned
        if (losingSound == null)
        {
            losingSound = Resources.Load<AudioClip>("losing");
        }
        
        // Ensure the person has a collider for collision detection
        if (GetComponent<Collider>() == null)
        {
            // Try to get collider from children
            Collider col = GetComponentInChildren<Collider>();
            if (col == null)
            {
                // Add a basic capsule collider if none exists
                CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 2f;
                capsule.radius = 0.5f;
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (gameEnded) return;
        
        CheckForChicken(other.gameObject);
        CollectGem(other.gameObject);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (gameEnded) return;
        
        CheckForChicken(collision.gameObject);
        CollectGem(collision.gameObject);
    }
    
    private void CollectGem(GameObject gemObject)
    {
        // Check if the object is a gem by checking its name
        if (IsGem(gemObject))
        {
            // Play reward sound
            PlayRewardSound();
            
            // Make the gem disappear
            Destroy(gemObject);
        }
    }
    
    private bool IsGem(GameObject obj)
    {
        string objName = obj.name;
        
        // Check if the object name starts with "gem" (case-insensitive)
        // This will match "gem", "gem 2", "gem3", "gem 4", etc.
        return objName.ToLower().StartsWith("gem");
    }
    
    private void PlayRewardSound()
    {
        if (rewardSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(rewardSound, rewardSoundVolume);
        }
    }
    
    private void CheckForChicken(GameObject obj)
    {
        // Check if the object is a chicken
        if (IsChicken(obj))
        {
            HandleChickenCollision();
        }
    }
    
    private bool IsChicken(GameObject obj)
    {
        string objName = obj.name;
        
        // Check if the object name contains "chicken" (case-insensitive)
        // This will match "Chicken", "ChickenPrefab", etc.
        return objName.ToLower().Contains("chicken");
    }
    
    private void HandleChickenCollision()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        // Play losing sound
        PlayLosingSound();
        
        // End the game
        EndGame();
    }
    
    private void PlayLosingSound()
    {
        if (losingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(losingSound, losingSoundVolume);
        }
    }
    
    private void EndGame()
    {
        // Pause the game
        Time.timeScale = 0f;
        
        // Optional: You can also reload the scene after a delay
        // Uncomment the line below if you want to reload the scene instead of pausing
        // StartCoroutine(ReloadSceneAfterDelay(2f));
    }
    
    // Optional method to reload the scene after playing the losing sound
    private System.Collections.IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
