using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private float speed = 300f;
    private float horizontalInput;
    private float verticalInput;
    private float jumpInput;
    private float jumpForce = 3;
    private Rigidbody playerRb;
    private float turn;
    private Animator playerAnimator;
    private bool canAttack = true;
    private float attackRate = 1f;
    public float hitPoints = 100;
    private float baseDamage = 20;
    public bool isAttacking = false;
    private bool canJump = true;
    public GameObject oneHandSword;
    public GameObject twoHandSword;
    public GameObject bow;
    private float damageMultiplier = 1;
    public bool isAlive = true;
    public GameObject arrow;
    private int equippedWeapon = 0;
    private Vector3 offset = new Vector3(0, 1.5f, 0f);
    public int healthPotions = 0;
    public float damage;
    public bool hasSilverKey = false;
    public bool hasGoldKey = false;
    public GameObject action;
    public GameObject listen;
    private float iFrames = 1;
    private bool canTakeDamage = true;
    private AudioSource playerAudio;
    public AudioClip attackSound;
    public AudioClip getHitSound;
    public AudioClip bowSound;
    public AudioClip drinkSound;
    public AudioClip buffSound;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAlive)
        {
            Move();
        }
    }
    void Update()
    {
        if (isAlive)
        {
            Rotate();
            Attack();
            AnimateMove();
            EquipWeapon();
            Act();
            CapHP();
            Heal();
        }
    }
    void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetAxis("Jump");
        if (jumpInput > 0 && canJump)
        {
            playerRb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }
        Vector3 velocity = ((transform.forward * verticalInput) + (transform.right * horizontalInput)) * speed * Time.fixedDeltaTime;
        velocity.y = playerRb.velocity.y;
        playerRb.velocity = velocity;
    }
    void Act()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Action(action));
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Action(listen));
        }
    }
    IEnumerator Action(GameObject objectToSet)
    {
        objectToSet.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        objectToSet.SetActive(false);
    }
    void AnimateMove()
    {
        playerAnimator.SetFloat("vertical_input", verticalInput);
        playerAnimator.SetFloat("horizontal_input", horizontalInput);
        if (jumpInput > 0 && canJump)
        {
            playerAnimator.SetTrigger("jump");
        }
        playerAnimator.SetFloat("jump_input", jumpInput);
    }
    void Rotate()
    {
        turn += Input.GetAxis("Mouse X");
        transform.localRotation = Quaternion.Euler(transform.rotation.x, turn, transform.rotation.y);
    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            isAttacking = true;
            canAttack = false;
            playerAnimator.SetTrigger("Attack");
            if (equippedWeapon == 3)
            {
                playerAudio.PlayOneShot(bowSound);
                Invoke("ShootArrow", 0.3f);
            } 
            else
            {
                playerAudio.PlayOneShot(attackSound);
            }
            //Debug.Log("Attak!");
            Invoke("ResetAttack", attackRate);
        }
    }
    void ShootArrow()
    {
        Instantiate(arrow, transform.position + offset, transform.rotation * Quaternion.Euler(-1.5f, 1, 1));
    }
    void ResetAttack()
    {
        canAttack = true;
        isAttacking= false;
    }
    public void GetDamaged(float damage)
    {
        if (isAlive && canTakeDamage)
        {
            playerAudio.PlayOneShot(getHitSound);
            hitPoints -= damage;
            canTakeDamage = false;
            Invoke("ResetIFrames", iFrames);
            if (hitPoints <= 0)
            {
                isAlive = false;
                playerAnimator.SetTrigger("die");
            }
            //else
            //{
            //    playerAnimator.SetTrigger("getHit");
            //}
        }
    }
    private void ResetIFrames()
    {
        canTakeDamage = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
        }
    }
    private void EquipWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            equippedWeapon = 1;
            playerAnimator.SetInteger("attackAnimation", equippedWeapon);
            oneHandSword.SetActive(true);
            twoHandSword.SetActive(false);
            bow.SetActive(false);
            attackRate = 1f;
            damageMultiplier = 1;
            damage = baseDamage * damageMultiplier;
            Debug.Log("One Hand Sword");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            equippedWeapon = 2;
            oneHandSword.SetActive(false);
            twoHandSword.SetActive(true);
            bow.SetActive(false);
            attackRate = 1.7f;
            damageMultiplier = 2;
            damage = baseDamage * damageMultiplier;
            playerAnimator.SetInteger("attackAnimation", equippedWeapon);
            Debug.Log("Two Hand Sword");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            equippedWeapon = 3;
            oneHandSword.SetActive(false);
            twoHandSword.SetActive(false);
            bow.SetActive(true);
            attackRate = 0.8f;
            damageMultiplier = 0.3f;
            damage = baseDamage * damageMultiplier;
            playerAnimator.SetInteger("attackAnimation", equippedWeapon);
            Debug.Log("Bow");
        }
    }
    public void IncreaseDamage()
    {
        baseDamage += 10;
        damage = baseDamage * damageMultiplier;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage boost"))
        {
            playerAudio.PlayOneShot(buffSound);
            IncreaseDamage();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Health Potion"))
        {
            playerAudio.PlayOneShot(buffSound);
            healthPotions += 1;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Silver Key"))
        {
            playerAudio.PlayOneShot(buffSound);
            hasSilverKey = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Gold Key"))
        {
            playerAudio.PlayOneShot(buffSound);
            hasGoldKey = true;
            Destroy (other.gameObject);
        }
    }
    void Heal()
    {
        if (Input.GetKeyDown(KeyCode.Q) && healthPotions >=0)
        {
            playerAudio.PlayOneShot(drinkSound);
            hitPoints += 30;
            healthPotions--;
        }
    }
    void CapHP()
    {
        if (hitPoints > 100)
        {
            hitPoints = 100;
        }
        else if (hitPoints < 0)
        {
            hitPoints = 0;
        }
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("hitPoints", hitPoints);
        PlayerPrefs.SetFloat("defaultDamage", damage);
        PlayerPrefs.SetInt("healthPotions", healthPotions);
        //PlayerPrefs.SetFloat("Defense", defense);
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "Prison")
        {
            hitPoints = 100;
            baseDamage = 20;
            healthPotions = 0;
            //defense = 10;
        }
        else
        {
            hitPoints = PlayerPrefs.GetFloat("hitPoints", 100);
            baseDamage = PlayerPrefs.GetFloat("defaultDamage", 20);
            healthPotions = PlayerPrefs.GetInt("healthPotions", 0);
            //defense = PlayerPrefs.GetFloat("Defense", 10);
        }
    }
}
