using UnityEngine;

internal class MobStats {
    internal float deathDelay;
    internal int damage, hp, defense, sight, speed, engangeRange;
    internal MobStats()
	{
        damage = 15;
        hp = 1000;
        defense = 0;
        sight = 50;
        speed = 250;
        deathDelay = 2f;
        engangeRange = 10;
    }
}
internal class MobComponents
{
    internal readonly Rigidbody body;
    internal readonly Animator animator;
    internal readonly GameObject enemy;
    internal readonly MobStats stat;
    internal MobComponents(Rigidbody rb, Animator an, GameObject pc)
    {
        body = rb;
        animator = an;
        enemy = pc;
        stat = new MobStats();
    }
}
public class MobBrain : MonoBehaviour
{
    public GameObject HealingPotion, Sword, Bow, Armor;
    MobComponents My;
    Entity I;


    public int Health() => My.stat.hp;
    void Start()
    {
        I = new Entity();
        My = new MobComponents(
            GetComponent<Rigidbody>(),
            GetComponent<Animator>(),
            GameObject.Find("Player")
            );
    }
    void Assess(Stance flag,int range)
	{
        if (Vector3.Distance(transform.position, My.enemy.transform.position) < range) I.Enter(flag);
        else I.Exit(flag);
    }
    void Update()
    {
        if (My.enemy is null || I.AmIn(Stance.Dieing)) return;
        Assess(Stance.inRange, My.stat.sight);
        Assess(Stance.Engaged, My.stat.engangeRange);
    }
    void FixedUpdate()
    {
        if (My.enemy is null || I.AmIn(Stance.Dieing)) return;
        if (I.AmNotIn(Stance.inRange) || !My.enemy.GetComponent<PlayerController>().IsAlive()) My.animator.SetBool("Walk Forward", false);
        else if(I.AmNotIn(Stance.Engaged) && My.enemy.GetComponent<PlayerController>().IsAlive())
		{
            Vector3 lookDirection = (My.enemy.transform.position - transform.position).normalized;
            transform.LookAt(My.enemy.transform);
            My.animator.SetBool("Walk Forward", true);
            My.body.rotation = Quaternion.Euler(0, My.body.rotation.eulerAngles.y, 0);
            My.body.AddForce(lookDirection * My.stat.speed);
		}
    }
	private void LateUpdate()
	{
		if (I.AmIn(Stance.Dieing)) Invoke("DropLoot", My.stat.deathDelay);
		else if (!My.enemy.GetComponent<PlayerController>().IsAlive())
		{
            SetAnimation("FullStop");
            CancelInvoke();
		}
        else if (My.stat.hp <= 0)
		{
            SetAnimation("FullStop");
            Invoke("Dieing", 0.1f);
        }
    }
    void Dieing()
	{
		if (I.AmNotIn(Stance.Dieing))
		{
            My.animator.SetTrigger("Die");
            I.Enter(Stance.Dieing);
		}
	}
    void DoDamage()
        {
            My.body.velocity = Vector3.zero;
            SetAnimation("Attacking");
            My.enemy.GetComponent<PlayerController>().GetDamaged(My.stat.damage);
        }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")&&!I.AmIn(Stance.Dieing)) InvokeRepeating("DoDamage", 0.1f, 0.5f);
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player Weapon") && My.enemy.GetComponent<PlayerController>().IsAttacking())
        {
            bool damaged = (My.enemy.GetComponent<PlayerController>().Damage() > My.stat.defense);
            if (damaged) My.stat.hp -= (int)(My.enemy.GetComponent<PlayerController>().Damage() - My.stat.defense);
            SetAnimation("Hurting");
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Player Weapon"))
        {
            CancelInvoke();
            SetAnimation("FullStop");
		}
	}
    void DropLoot()
    {
        SetAnimation("FullStop");
		Destroy(gameObject);
        float chance = (Random.Range(0, 1000)%4)/4;
        if (chance <= 0.25) Instantiate(HealingPotion, transform.position, transform.rotation);
        if (chance > 0.25 && chance <= 0.5) Instantiate(Sword, transform.position, transform.rotation);
        if (chance > 0.5 && chance <= 0.75) Instantiate(Bow, transform.position, transform.rotation);
        if (chance > 0.75) Instantiate(Armor, transform.position, transform.rotation);
    }
    void SetAnimation(string state)
	{
		switch (state)
		{
            case "FullStop":
                My.animator.SetBool("Walk Forward", false);
                My.animator.ResetTrigger("Stab Attack");
                My.animator.ResetTrigger("Take Damage");
                break;
            case "Attacking":
                My.animator.SetBool("Walk Forward", false);
                My.animator.SetTrigger("Stab Attack");
                My.animator.ResetTrigger("Take Damage");
                break;
            case "Hurting":
                My.animator.SetBool("Walk Forward", false);
                My.animator.ResetTrigger("Stab Attack");
                My.animator.SetTrigger("Take Damage");
                break;
        }
    }
}
