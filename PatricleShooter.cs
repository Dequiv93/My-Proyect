using UnityEngine;

public class PatricleShooter : MonoBehaviour
{
    public ParticleSystem myParticleSystemPrefab; // Reference to the prefab
    private ParticleSystem myParticleSystem; // Instance of the particle system
    public float shootInterval = 1f; // Time between shots
    private float nextShootTime = 0f; // Time when the next shot can occur

    private void Start()
    {
        // Instantiate the Particle System if it's not assigned
        if (myParticleSystemPrefab != null)
        {
            myParticleSystem = Instantiate(myParticleSystemPrefab, transform.position, Quaternion.identity);
            myParticleSystem.Stop(); // Stop it initially, so it doesn't play on start
        }
        else
        {
            Debug.LogError("Particle System prefab is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // Check if it's time to shoot
        if (Time.time >= nextShootTime)
        {
            // Check for input (e.g., space bar)
            if (Input.GetKeyDown(KeyCode.Y))
            {
                ShootParticles();
                nextShootTime = Time.time + shootInterval; // Set the next shoot time
            }
            if (myParticleSystem == null)
            {
                Debug.LogWarning("Particle System reference lost!");
                // Optionally, you can try to reassign it here if needed
            }
        }

    }

    void ShootParticles()
    {
        if (myParticleSystem != null)
        {
            myParticleSystem.transform.position = transform.position; // Set position to the character's head
            myParticleSystem.Play(); // Play the particle system
        }
        else
        {
            Debug.LogWarning("Particle System is not assigned!");
        }
    }
}
