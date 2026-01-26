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
    public int totalGems = 4; // Total number of gems to collect
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 90f; // Degrees per second
    public bool overrideMouseLook = true; // If true, movement input overrides mouse look
    
    [Header("OptiTrack Motion Tracking")]
    public bool useOptiTrack = true; // Enable OptiTrack motion tracking
    public int optiTrackRigidBodyId = 1; // The Rigid Body ID in Motive (set this to match your tracked object)
    
    private AudioSource audioSource;
    private bool gameEnded = false;
    private FirstPersonLook firstPersonLook;
    private OptitrackRigidBody optiTrackRigidBody;
    private FirstPersonMovement firstPersonMovement;
    private bool isOptiTrackActive = false;
    private int gemsCollected = 0; // Track number of gems collected
    
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
        
        // Try to load winning sound if not assigned
        if (winningSound == null)
        {
            winningSound = Resources.Load<AudioClip>("mixkit-winning-chimes-2015");
        }
        
        // Initialize gem counter
        gemsCollected = 0;
        
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
        
        // Try to find FirstPersonLook component (might be on a child camera)
        firstPersonLook = GetComponentInChildren<FirstPersonLook>();
        if (firstPersonLook == null)
        {
            firstPersonLook = GetComponent<FirstPersonLook>();
        }
        
        // Find FirstPersonMovement component to disable when OptiTrack is active
        firstPersonMovement = GetComponent<FirstPersonMovement>();
        
        // Setup OptiTrack motion tracking
        SetupOptiTrack();
    }
    
    private void SetupOptiTrack()
    {
        if (!useOptiTrack)
        {
            // OptiTrack is disabled - enable keyboard controls
            isOptiTrackActive = false;
            
            // Remove OptiTrack component if it exists
            if (optiTrackRigidBody != null)
            {
                Destroy(optiTrackRigidBody);
                optiTrackRigidBody = null;
            }
            
            // Enable keyboard movement controls
            if (firstPersonMovement != null)
            {
                firstPersonMovement.enabled = true;
                Debug.Log("OptiTrack: Disabled. Keyboard controls enabled.");
            }
            return;
        }
        
        // OptiTrack is enabled - setup tracking
        // Check if OptitrackRigidBody component already exists
        optiTrackRigidBody = GetComponent<OptitrackRigidBody>();
        
        if (optiTrackRigidBody == null)
        {
            // Add OptitrackRigidBody component
            optiTrackRigidBody = gameObject.AddComponent<OptitrackRigidBody>();
            optiTrackRigidBody.RigidBodyId = optiTrackRigidBodyId;
            
            Debug.Log($"OptiTrack: Added OptitrackRigidBody component to {gameObject.name} with RigidBodyId {optiTrackRigidBodyId}");
        }
        else
        {
            // Update the RigidBodyId if it was changed
            optiTrackRigidBody.RigidBodyId = optiTrackRigidBodyId;
        }
        
        // Check if OptiTrack client exists in scene
        OptitrackStreamingClient client = OptitrackStreamingClient.FindDefaultClient();
        if (client == null)
        {
            Debug.LogWarning("OptiTrack: No OptitrackStreamingClient found in scene. Please add the 'Client - OptiTrack' prefab to your scene.");
            isOptiTrackActive = false;
            
            // Enable keyboard controls if OptiTrack client not found
            if (firstPersonMovement != null)
            {
                firstPersonMovement.enabled = true;
            }
        }
        else
        {
            isOptiTrackActive = true;
            Debug.Log("OptiTrack: Client found. Motion tracking will control position and rotation.");
            
            // Disable conflicting movement components
            if (firstPersonMovement != null)
            {
                firstPersonMovement.enabled = false;
                Debug.Log("OptiTrack: Disabled FirstPersonMovement component.");
            }
        }
    }
    
    void Update()
    {
        if (gameEnded) return;
        
        // Only handle keyboard rotation if OptiTrack is not active
        if (!isOptiTrackActive)
        {
            HandleArrowKeyRotation();
        }
    }
    
    void LateUpdate()
    {
        if (gameEnded) return;
        
        // Only apply movement rotation if OptiTrack is not active
        // OptiTrack will control position and rotation when active
        if (!isOptiTrackActive)
        {
            ApplyMovementRotation();
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
            // Increment gem counter
            gemsCollected++;
            
            // Play reward sound
            PlayRewardSound();
            
            // Make the gem disappear
            Destroy(gemObject);
            
            // Check if all gems are collected
            if (gemsCollected >= totalGems)
            {
                HandleWinCondition();
            }
        }
    }
    
    private void HandleWinCondition()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        // Play winning sound
        PlayWinningSound();
        
        // End the game
        EndGame();
    }
    
    private void PlayWinningSound()
    {
        if (winningSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(winningSound, winningSoundVolume);
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
    
    private void HandleArrowKeyRotation()
    {
        // This method is kept for compatibility but rotation is now handled in LateUpdate
    }
    
    private void ApplyMovementRotation()
    {
        // Get movement input (works with both WASD and Arrow keys)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Check if there's any movement input
        bool hasMovementInput = Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        
        if (hasMovementInput)
        {
            // Calculate direction from input axes (in world space)
            Vector3 targetDirection = new Vector3(horizontal, 0, vertical).normalized;
            
            // Only rotate if there's a meaningful direction
            if (targetDirection.magnitude > 0.1f)
            {
                // Calculate target rotation in world space
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                
                // Get current rotation
                Vector3 currentEuler = transform.eulerAngles;
                float currentY = currentEuler.y;
                float targetY = targetRotation.eulerAngles.y;
                
                // Calculate the shortest rotation path
                float angleDiff = Mathf.DeltaAngle(currentY, targetY);
                
                // Apply rotation only on Y axis (horizontal rotation)
                // This will override any mouse look rotation applied in Update
                float newY = currentY + Mathf.Sign(angleDiff) * Mathf.Min(Mathf.Abs(angleDiff), rotationSpeed * Time.deltaTime);
                
                // Preserve X and Z rotations (for camera pitch, etc.)
                transform.rotation = Quaternion.Euler(currentEuler.x, newY, currentEuler.z);
            }
        }
    }
    
    // Optional method to reload the scene after playing the losing sound
    private System.Collections.IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
