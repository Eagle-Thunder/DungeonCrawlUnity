using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 2;
    private PlayerController player;

    void Start() => player = GameObject.Find("Player").GetComponent<PlayerController>();
    private void OnTriggerEnter(Collider other)
    {
        Enemy owner = gameObject.GetComponentInParent<Enemy>();
        if (other.CompareTag("Player") && owner.IsAttacking())
        {
            player.GetDamaged(damage);
            Debug.Log("Player took " + damage + " damage");
        }
    }
}
