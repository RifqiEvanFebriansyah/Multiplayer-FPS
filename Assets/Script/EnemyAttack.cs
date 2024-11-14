using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public Health playerHealth;
    public int damageAmount = 20;

    void Update()
    {
        // For testing, press the H key to deal damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
