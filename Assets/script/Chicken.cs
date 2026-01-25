using UnityEngine;

public class Chicken : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    
    private Rigidbody rb;
    
    void Start()
    {
        // Get or add Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Ensure the chicken has a collider for collision detection
        if (GetComponent<Collider>() == null)
        {
            // Try to get collider from children, or add a basic one
            Collider col = GetComponentInChildren<Collider>();
            if (col == null)
            {
                CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 1f;
                capsule.radius = 0.3f;
            }
        }
    }

    void FixedUpdate()
    {
        // Move forward automatically
        Vector3 forwardMovement = transform.forward * moveSpeed;
        rb.linearVelocity = new Vector3(forwardMovement.x, rb.linearVelocity.y, forwardMovement.z);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a fence
        if (collision.gameObject.name.Contains("Fence"))
        {
            // Turn 180 degrees away from the fence
            transform.Rotate(0, 180, 0);
        }
    }
}
