using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 2;
    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = gameObject.GetComponentInParent<Enemy>();
        if (other.CompareTag("Player") && enemy.isAttacking)
        {
            player.GetDamaged(damage);
            Debug.Log("Player took " + damage + " damage");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
    }
}
