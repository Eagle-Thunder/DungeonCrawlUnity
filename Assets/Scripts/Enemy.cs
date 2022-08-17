using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hitpoints = 100;
    private PlayerController player;
    private float iFrames = 0.7f;
    private bool canTakeDamage = true;
    private Rigidbody enemyRb;
    public float speed = 4f;
    private float playerProximity = 1;
    private float aggroRange = 10;
    private Animator enemyAnimator;
    private float attackRate = 1.3f;
    private bool canAttack = true;
    public bool isAttacking = false;
    public int attackAnimation = 1;
    private bool isAlive = true;
    private bool isAggressive = false;
    public GameObject[] drops;
    private AudioSource enemyAudio;
    public AudioClip getHitSound;
    public AudioClip attackSound;
    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        enemyAnimator.SetInteger("attackAnimation", attackAnimation);
    }

    private void FixedUpdate()
    {
        if (isAlive && player.isAlive && isAggressive)
        {
            if (IsInRange())
            {
                Stop();
            }
            else
            {
                moveToPlayer();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckAggro();
        if (isAlive && player.isAlive && isAggressive)
        {
            if (IsInRange() && canAttack)
            {
                Attack();
            }
            Rotate();
        }
        AnimateMovement();
    }
    void AnimateMovement()
    {
        enemyAnimator.SetFloat("vertical_input", enemyRb.velocity.z);
        enemyAnimator.SetFloat("horizontal_input", enemyRb.velocity.x);
    }
    void Rotate()
    {
        transform.LookAt(player.transform.position);
    }
    void Attack()
    {
        isAttacking = true;
        canAttack = false;
        enemyAnimator.SetTrigger("Attack");
        Invoke("ResetAttack", attackRate);
        enemyAudio.PlayOneShot(attackSound);
    }
    bool IsInRange()
    {
        float xProximity = Mathf.Abs(player.transform.position.x - transform.position.x);
        float zProximity = Mathf.Abs(player.transform.position.z - transform.position.z);
        if (xProximity <= playerProximity && zProximity <= playerProximity)
        {
            return true;
        }
        return false;
    }
    void Stop()
    {
        enemyRb.velocity = Vector3.zero;
    }
    void CheckAggro()
    {
        float xProximity = Mathf.Abs(player.transform.position.x - transform.position.x);
        float zProximity = Mathf.Abs(player.transform.position.z - transform.position.z);
        if (xProximity <= aggroRange && zProximity <= aggroRange)
        {
            isAggressive = true;
        }
        //else isAggressive = false;
    }
    void moveToPlayer()
    {
        //enemyRb.AddForce((player.transform.position - transform.position).normalized * speed * Time.deltaTime, ForceMode.Impulse);
        Vector3 velocity = (player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        velocity.y = enemyRb.velocity.y;
        enemyRb.velocity = velocity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Weapon") && canTakeDamage && player.isAttacking)
        {
            hitpoints -= player.damage;
            enemyAudio.PlayOneShot(getHitSound);
            isAggressive = true;
            if (isAlive)
            {
                if (hitpoints <= 0)
                {
                    isAlive = false;
                    enemyAnimator.SetTrigger("die");
                    DropLoot();
                }
                else
                {
                    enemyAnimator.SetTrigger("getHit");
                }
            }
            canTakeDamage = false;
            Debug.Log("Enemy has " + hitpoints + " left!");
            Invoke("ResetIFrames", iFrames);
        }
    }

    private void ResetIFrames()
    {
        canTakeDamage = true;
    }
    void ResetAttack()
    {
        canAttack = true;
        isAttacking = false;
    }
    void DropLoot()
    {
        float dropRate = Random.Range(0, 101);
        if (dropRate <= 25)
        {
            int index = Random.Range(0, 1);
            Instantiate(drops[index], transform.position + Vector3.up, transform.rotation);
        }
    }
}
