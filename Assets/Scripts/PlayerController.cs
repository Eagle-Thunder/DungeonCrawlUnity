using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

internal enum Stance : int
{
    Engaged = 1,
    Damageable = 2,
    AttackReady = 4,
    Alive = 8,
    Aggressive = 16,
    Agro,
    InRange = 32,
    Dieing = 64,
    inRange = 128,
    JumpAble = 256
}
internal enum Inventory:int
{
    SilverKey = 1,
    GoldKey = 2,
}
internal enum Sound : int
{
    attack,
    getHit,
    bow,
    drink,
    buff
}
internal enum Item : int
{
    oneHandSword,
    twoHandSword,
    bow,
    arrow,
    action,
	listen
}
internal class Entity
{
    private int stance = 0, inventory = 0;
    internal bool AmIn(Stance s) => (stance & (int)s) != 0;
    internal bool AmNotIn(Stance s) => (stance & (int)s) == 0;
    internal void Enter(Stance s) => stance |= (int)s;
    internal void Exit(Stance s) => stance &= ~(int)s;
    internal bool HaveIn(Inventory s) => (inventory & (int)s) != 0;
    internal bool HaveNotIn(Inventory s) => (inventory & (int)s) == 0;
    internal void Get(Inventory s) => inventory |= (int)s;
    internal void Loose(Inventory s) => inventory &= ~(int)s;
}
internal class PlayerStats
{
    internal float speed, jumpForce, attackRate, baseDamage, damageMultiplier;
    internal float iFrames, offset, horizontalInput, verticalInput, jumpInput, turn;
    internal int equippedWeapon;
    public int healthPotions;
    public float damage, hitPoints;
    internal PlayerStats()
    {
        speed = 300f;
        jumpForce = 3;
        attackRate = 1f;
        baseDamage = 20;
        damageMultiplier = 1;
        iFrames = 1;
        offset = 1.5f;
        equippedWeapon = 0;
        healthPotions = 0;
        hitPoints = 100;
    }
}
internal class PlayerComponents
{
    internal readonly Rigidbody body;
    internal readonly Animator animator;
    internal readonly AudioSource voice;
    internal readonly PlayerStats stat;
    internal PlayerComponents(Rigidbody rb, Animator an, AudioSource pa)
    {
        body = rb;
        animator = an;
        voice = pa;
        stat = new PlayerStats();
    }
}
public class PlayerController : MonoBehaviour
{
    public List<GameObject> item;
    public List<AudioClip> say;
	Entity I;
    PlayerComponents My;

	void Start()
    {
        I = new Entity();
        My=new PlayerComponents(
            GetComponent<Rigidbody>(),
            GetComponent<Animator>(),
            GetComponent<AudioSource>()
            );
        I.Enter(Stance.Alive);
        I.Enter(Stance.Damageable);
        I.Enter(Stance.JumpAble);
        I.Enter(Stance.AttackReady);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void FixedUpdate()
    {
        if (I.AmNotIn(Stance.Alive)) return;
        My.stat.horizontalInput = Input.GetAxis("Horizontal");
        My.stat.verticalInput = Input.GetAxis("Vertical");
        My.stat.jumpInput = Input.GetAxis("Jump");
        if (My.stat.jumpInput > 0 && I.AmIn(Stance.JumpAble)) My.body.AddForce(transform.up * My.stat.jumpForce, ForceMode.VelocityChange);
        Vector3 velocity = My.stat.speed * Time.fixedDeltaTime * ((transform.forward * My.stat.verticalInput) + (transform.right * My.stat.horizontalInput));
        velocity.y = My.body.velocity.y;
        My.body.velocity = velocity;
    }
    void Update()
    {
        if (I.AmNotIn(Stance.Alive)) return;
        Rotate();
        Attack();
        AnimateMove();
        EquipWeapon();
        Act();
        CapHP();
        Heal();
    }
    public int HealthPotions() => My.stat.healthPotions;
    public float HitPoints() => My.stat.hitPoints;
    public float Damage() => My.stat.damage;
    public bool IsAttacking() => I.AmIn(Stance.Engaged);
    public bool IsAlive() => I.AmIn(Stance.Alive);
    public bool HasSilverKey() => I.HaveIn(Inventory.SilverKey);
    public bool HasGoldKey() => I.HaveIn(Inventory.GoldKey);
    void Act()
    {
        if (Input.GetKeyDown(KeyCode.E)) StartCoroutine(Action(item[(int)Item.action]));
        else if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(Action(item[(int)Item.listen]));
    }
    IEnumerator Action(GameObject objectToSet)
    {
        objectToSet.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        objectToSet.SetActive(false);
    }
    void AnimateMove()
    {
        My.animator.SetFloat("vertical_input", My.stat.verticalInput);
        My.animator.SetFloat("horizontal_input", My.stat.horizontalInput);
        if (My.stat.jumpInput > 0 && I.AmIn(Stance.JumpAble)) My.animator.SetTrigger("jump");
        My.animator.SetFloat("jump_input", My.stat.jumpInput);
    }
    void Rotate()
    {
        My.stat.turn += Input.GetAxis("Mouse X");
        transform.localRotation = Quaternion.Euler(transform.rotation.x, My.stat.turn, transform.rotation.y);
    }
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && I.AmIn(Stance.AttackReady))
        {
            I.Enter(Stance.Engaged);
            I.Exit(Stance.AttackReady);
            My.animator.SetTrigger("Attack");
            if (My.stat.equippedWeapon == 3)
            {
                My.voice.PlayOneShot(say[(int)Sound.bow]);
                Invoke("ShootArrow", 0.3f);
            } 
            else My.voice.PlayOneShot(say[(int)Sound.attack]);
            Invoke("ResetAttack", My.stat.attackRate);
        }
    }
    void ShootArrow() => Instantiate(item[(int)Item.arrow], transform.position + Vector3.up*My.stat.offset, transform.rotation * Quaternion.Euler(-1.5f, 1, 1));
    void ResetAttack()
    {
        I.Enter(Stance.AttackReady);
        I.Exit(Stance.Engaged);
    }
    public void GetDamaged(float damage)
    {
        if (I.AmNotIn(Stance.Alive) || I.AmNotIn(Stance.Damageable)) return;
        My.voice.PlayOneShot(say[(int)Sound.getHit]);
        My.stat.hitPoints -= damage;
        I.Exit(Stance.Damageable);
        Invoke("ResetIFrames", My.stat.iFrames);
        if (My.stat.hitPoints <= 0)
        {
            I.Exit(Stance.Alive);
            My.animator.SetTrigger("die");
        }
    }
    private void ResetIFrames() => I.Enter(Stance.Damageable);
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) I.Enter(Stance.JumpAble);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) I.Exit(Stance.JumpAble);
    }
    private void SetWepon(Item weapon,float rating,float multiplier)
	{
            for(int i=(int)Item.oneHandSword; i<=(int)Item.bow; i++) item[i].SetActive(false);
            item[(int)weapon].SetActive(true);
            My.stat.equippedWeapon = (int)weapon+1;
            My.animator.SetInteger("attackAnimation", My.stat.equippedWeapon);
            My.stat.attackRate = rating;
            My.stat.damageMultiplier = multiplier;
            My.stat.damage = My.stat.baseDamage * My.stat.damageMultiplier;
	}
    private void EquipWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetWepon(Item.oneHandSword, 1f, 1f);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetWepon(Item.twoHandSword, 1.7f, 2f);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SetWepon(Item.bow,0.8f,0.3f);
    }
    public void IncreaseDamage()
    {
        My.stat.baseDamage += 10;
        My.stat.damage = My.stat.baseDamage * My.stat.damageMultiplier;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damage boost"))
        {
            My.voice.PlayOneShot(say[(int)Sound.buff]);
            IncreaseDamage();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Health Potion"))
        {
            My.voice.PlayOneShot(say[(int)Sound.buff]);
            My.stat.healthPotions += 1;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Silver Key"))
        {
            My.voice.PlayOneShot(say[(int)Sound.buff]);
            I.Get(Inventory.SilverKey);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Gold Key"))
        {
            My.voice.PlayOneShot(say[(int)Sound.buff]);
            I.Get(Inventory.GoldKey);
            Destroy (other.gameObject);
        }
    }
    void Heal()
    {
        if (!Input.GetKeyDown(KeyCode.Q) || My.stat.healthPotions < 0) return;
        My.voice.PlayOneShot(say[(int)Sound.drink]);
        My.stat.hitPoints += 30;
        My.stat.healthPotions--;
    }
    void CapHP()
    {
        if (My.stat.hitPoints > 100) My.stat.hitPoints = 100;
        else if (My.stat.hitPoints < 0) My.stat.hitPoints = 0;
    }
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("hitPoints", My.stat.hitPoints);
        PlayerPrefs.SetFloat("defaultDamage", My.stat.damage);
        PlayerPrefs.SetInt("healthPotions", My.stat.healthPotions);
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "Prison")
        {
            My.stat.hitPoints = 100;
            My.stat.baseDamage = 20;
            My.stat.healthPotions = 0;
        }
        else
        {
            My.stat.hitPoints = PlayerPrefs.GetFloat("hitPoints", 100);
            My.stat.baseDamage = PlayerPrefs.GetFloat("defaultDamage", 20);
            My.stat.healthPotions = PlayerPrefs.GetInt("healthPotions", 0);
        }
    }
}
