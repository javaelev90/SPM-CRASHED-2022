
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private PlayerHealthBarHandler healthBarHandler;

    [Header("Shoot")]
    [SerializeField] private float shootRange;
    [SerializeField] private float shootCooldown;
    public int Damage { set; get; }

    private float shootCooldownTimer = 0;
    


    public void Shoot()
    {
        if (shootCooldownTimer <= 0)
        {
            // TODO: shoot
            shootCooldownTimer = shootCooldown;
        }
    }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBarHandler.SetHealthBarValue(currentHealth / maxHealth);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown();
    }

    private void CoolDown()
    {
        if (shootCooldownTimer > 0)
        {
            shootCooldownTimer -= Time.deltaTime;
        }
    }
}
