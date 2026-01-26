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

    [Header("Rotation Settings")]
    public float rotationSpeed = 90f;
    public bool overrideMouseLook = true;

    [Header("OptiTrack Motion Tracking")]
    public bool useOptiTrack = true;
    public int optiTrackRigidBodyId = 1;

    private AudioSource audioSource;
    private bool gameEnded = false;
    private bool hasPlayedWinSound = false;

    private FirstPersonLook firstPersonLook;
    private OptitrackRigidBody optiTrackRigidBody;
    private FirstPersonMovement firstPersonMovement;
    private bool isOptiTrackActive = false;

    private int gemsCollected = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = rewardSoundVolume;
        }

        if (rewardSound == null)
            rewardSound = Resources.Load<AudioClip>("reward");

        if (losingSound == null)
            losingSound = Resources.Load<AudioClip>("losing");

        if (winningSound == null)
            winningSound = Resources.Load<AudioClip>("mixkit-winning-chimes-2015");

        gemsCollected = 0;

        if (GetComponent<Collider>() == null)
        {
            Collider col = GetComponentInChildren<Collider>();
            if (col == null)
            {
                CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 2f;
                capsule.radius = 0.5f;
            }
        }

        firstPersonLook = GetComponentInChildren<FirstPersonLook>();
        if (firstPersonLook == null)
            firstPersonLook = GetComponent<FirstPersonLook>();

        firstPersonMovement = GetComponent<FirstPersonMovement>();

        SetupOptiTrack();
    }

    private void SetupOptiTrack()
    {
        if (!useOptiTrack)
        {
            isOptiTrackActive = false;

            if (optiTrackRigidBody != null)
                Destroy(optiTrackRigidBody);

            if (firstPersonMovement != null)
                firstPersonMovement.enabled = true;

            return;
        }

        optiTrackRigidBody = GetComponent<OptitrackRigidBody>();
        if (optiTrackRigidBody == null)
        {
            optiTrackRigidBody = gameObject.AddComponent<OptitrackRigidBody>();
            optiTrackRigidBody.RigidBodyId = optiTrackRigidBodyId;
        }

        OptitrackStreamingClient client = OptitrackStreamingClient.FindDefaultClient();
        if (client == null)
        {
            isOptiTrackActive = false;
            if (firstPersonMovement != null)
                firstPersonMovement.enabled = true;
        }
        else
        {
            isOptiTrackActive = true;
            if (firstPersonMovement != null)
                firstPersonMovement.enabled = false;
        }
    }

    void Update()
    {
        if (gameEnded) return;

        if (!isOptiTrackActive)
            HandleArrowKeyRotation();
    }

    void LateUpdate()
    {
        if (gameEnded) return;

        if (!isOptiTrackActive)
            ApplyMovementRotation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameEnded) return;

        CheckForChicken(other.gameObject);
        CollectGem(other.gameObject);
    }

    private void CollectGem(GameObject gemObject)
    {
        if (!IsGem(gemObject)) return;

        // prevent double-counting before Destroy()
        if (!gemObject.activeSelf) return;
        gemObject.SetActive(false);

        gemsCollected++;
        Debug.Log("Gems collected: " + gemsCollected);

        PlayRewardSound();
        Destroy(gemObject);

        // Play win sound exactly once when reaching total
        if (gemsCollected == totalGems)
        {
            HandleWinCondition();
        }
    }

    private void HandleWinCondition()
    {
        if (hasPlayedWinSound) return;

        hasPlayedWinSound = true;
        Debug.Log("All gems collected! Win sound played.");

        PlayWinningSound();
        // 🚫 Game does NOT end
    }

    private void PlayWinningSound()
    {
        if (winningSound != null && audioSource != null)
            audioSource.PlayOneShot(winningSound, winningSoundVolume);
    }

    private bool IsGem(GameObject obj)
    {
        return obj.name.ToLower().StartsWith("gem");
    }

    private void PlayRewardSound()
    {
        if (rewardSound != null && audioSource != null)
            audioSource.PlayOneShot(rewardSound, rewardSoundVolume);
    }

    private void CheckForChicken(GameObject obj)
    {
        if (IsChicken(obj))
            HandleChickenCollision();
    }

    private bool IsChicken(GameObject obj)
    {
        return obj.name.ToLower().Contains("chicken");
    }

    private void HandleChickenCollision()
    {
        if (gameEnded) return;

        gameEnded = true;
        PlayLosingSound();
        EndGame();
    }

    private void PlayLosingSound()
    {
        if (losingSound != null && audioSource != null)
            audioSource.PlayOneShot(losingSound, losingSoundVolume);
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
    }

    private void HandleArrowKeyRotation() { }

    private void ApplyMovementRotation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            Vector3 dir = new Vector3(horizontal, 0, vertical).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(dir);

            float currentY = transform.eulerAngles.y;
            float targetY = targetRotation.eulerAngles.y;
            float angleDiff = Mathf.DeltaAngle(currentY, targetY);

            float newY = currentY + Mathf.Sign(angleDiff) *
                         Mathf.Min(Mathf.Abs(angleDiff), rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, newY, transform.eulerAngles.z);
        }
    }
}
