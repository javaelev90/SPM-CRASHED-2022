
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private HealthHandler healthHandler;

    [Header("Shoot")]
    [SerializeField] private int damage;
    [SerializeField] private float shootRange; // Not used at the moment
    [SerializeField] private float shootCooldown;
    private float shootCooldownTimer = 0;

    public void Shoot()
    {
        if (shootCooldownTimer <= 0) // Check if we are ready to shoot
        {
            Debug.Log("Shots fired");

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit)) // What do we hit
            {
                Debug.Log("I hit something!!");

                if (hit.transform.gameObject.CompareTag("Enemy")) // If its an enemy we deal damage to it
                {
                    Debug.Log("DIE ENEMY DIE!");
                    hit.transform.gameObject.GetComponent<HealthHandler>().TakeDamage(damage);
                }
            }

            shootCooldownTimer = shootCooldown; // Start shoot cooldown
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healthHandler.MaxHealth = maxHealth;
        healthHandler.CurrentHealth = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown();
    }

    private void CoolDown()
    {
        if (shootCooldownTimer > 0) // If cooldown is active
        {
            shootCooldownTimer -= Time.deltaTime; // Reduce cooldown timer
        }
    }
}
