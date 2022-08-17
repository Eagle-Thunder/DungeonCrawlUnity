using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//prototype4
public class MobBrain : MonoBehaviour
{
    private Rigidbody body;
    private Animator animator;
    private GameObject player;
    public GameObject HealingPotion, Sword, Bow, Armor;
    public float speed, deathDelay, engangeRange;
    public int damage, hp, defense, sight;
    public bool InRange, Engaged, dieing;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        damage = 15;
        hp = 1000;
        defense = 0;
        sight = 50;
        speed = 250;
        deathDelay = 2f;
        engangeRange=10f;
        InRange = false;
        Engaged = false;
        dieing = false;
    }
    void Update()
    {
        if (player is null || dieing) return;
        InRange = (Mathf.Abs(transform.position.x - player.transform.position.x) < sight
                && Mathf.Abs(transform.position.z - player.transform.position.z) < sight);
        Engaged = (Mathf.Abs(transform.position.x - player.transform.position.x) < engangeRange
                && Mathf.Abs(transform.position.z - player.transform.position.z) < engangeRange);
    }
    void FixedUpdate()
    {
        if (player is null || dieing) return;
        if (!InRange|| !player.GetComponent<PlayerController>().isAlive)
        {
            animator.SetBool("Walk Forward", false);
            return;
        }
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        transform.LookAt(player.transform);
        if(!Engaged && player.GetComponent<PlayerController>().isAlive) 
            animator.SetBool("Walk Forward", true);
        body.rotation = Quaternion.Euler(0, body.rotation.eulerAngles.y, 0);
        body.AddForce(lookDirection * speed);
    }
	private void LateUpdate()
	{
		if (dieing) Invoke("DropLoot", deathDelay);
		else if (!player.GetComponent<PlayerController>().isAlive)
		{
            CancelInvoke();
            animator.SetBool("Walk Forward", false);
            animator.ResetTrigger("Stab Attack");
            animator.ResetTrigger("Take Damage");
		}
        else if (hp <= 0)
		{
			animator.SetBool("Walk Forward", false);
			animator.ResetTrigger("Take Damage");
			animator.ResetTrigger("Stab Attack");
            Invoke("Dieing", 0.1f);
        }
    }
    void Dieing()
	{
		if (!dieing)
		{
            Debug.Log("Dieing");
            animator.SetTrigger("Die");
            dieing = true;
		}
	}
    void DoDamage()
        {
            body.velocity = new Vector3(0, 0, 0);
            animator.SetBool("Walk Forward", false);
            animator.SetTrigger("Stab Attack");
		    animator.ResetTrigger("Take Damage");
		    player.GetComponent<PlayerController>().GetDamaged(damage);
        }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")&&!dieing) InvokeRepeating("DoDamage", 0.1f, 0.5f);
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player Weapon") && player.GetComponent<PlayerController>().isAttacking)
        {
            bool damaged = (player.GetComponent<PlayerController>().damage > defense);
            if (damaged) hp -= (int)(player.GetComponent<PlayerController>().damage - defense);
            animator.SetBool("Walk Forward", false);
			animator.ResetTrigger("Stab Attack");
			animator.SetTrigger("Take Damage");
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Player Weapon"))
        {
            CancelInvoke();
			animator.ResetTrigger("Stab Attack");
			animator.ResetTrigger("Take Damage");
		}
	}
    void DropLoot()
    {
        Debug.Log("Dropping");
		animator.ResetTrigger("Stab Attack");
		animator.ResetTrigger("Take Damage");
		Destroy(gameObject);
        float chance = Random.Range(0, 1);
        if (chance <= 0.25) Instantiate(HealingPotion, transform.position, transform.rotation);
        if (chance > 0.25 && chance <= 0.5) Instantiate(Sword, transform.position, transform.rotation);
        if (chance > 0.5 && chance <= 0.75) Instantiate(Bow, transform.position, transform.rotation);
        if (chance > 0.75) Instantiate(Armor, transform.position, transform.rotation);
    }
}
