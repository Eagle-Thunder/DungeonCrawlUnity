using UnityEngine;

internal class EnemyStats
{
    internal int attackAnimation;
    internal float hitpoints;
    internal float speed;
    internal float iFrames;
    internal float playerProximity;
    internal float aggroRange;
    internal float attackRate;
	internal EnemyStats() {
        attackAnimation = 1;
        hitpoints = 100;
        speed = 4f;
        iFrames = 0.7f;
        playerProximity = 1;
        aggroRange = 10;
        attackRate = 1.3f;
}
}
internal class EnemyComponents
{
    internal readonly AudioSource Audio;
    internal readonly PlayerController enemy;
    internal readonly Rigidbody body;
    internal readonly Animator animator;
    internal readonly EnemyStats stat;
    internal EnemyComponents(Rigidbody rb,Animator an,AudioSource AS,PlayerController pc)
	{
        body = rb;
        animator = an;
        Audio = AS;
        enemy = pc;
        stat = new EnemyStats();
	}
}
public class Enemy : MonoBehaviour
{
    public GameObject[] drops;
    public AudioClip getHitSound;
    public AudioClip attackSound;
    EnemyComponents My;
    Entity I;

    public bool IsAttacking() => I.AmIn(Stance.Engaged);
    void Start()
    {
        I = new Entity();
        My = new EnemyComponents(
            GetComponent<Rigidbody>(),
            GetComponent<Animator>(),
            GetComponent<AudioSource>(),
            GameObject.Find("Player").GetComponent<PlayerController>()
            );
        My.animator.SetInteger("attackAnimation", My.stat.attackAnimation);
    }
    private void FixedUpdate()
    {
        if (I.AmIn(Stance.Agro) && My.enemy.IsAlive())
        {
            if (I.AmIn(Stance.InRange)) Stop();
            else MoveToPlayer();
        }
    }
    void Update()
    {
        Assess(Stance.InRange, My.stat.playerProximity);
        Assess(Stance.Aggressive, My.stat.aggroRange);
        if (I.AmIn(Stance.Agro) && My.enemy.IsAlive())
        {
            if (I.AmIn(Stance.InRange) && I.AmIn(Stance.AttackReady)) Attack();
            Rotate();
        }
        AnimateMovement();
    }
    void AnimateMovement()
    {
        My.animator.SetFloat("vertical_input", My.body.velocity.z);
        My.animator.SetFloat("horizontal_input", My.body.velocity.x);
    }
    void Rotate() => transform.LookAt(My.enemy.transform.position);
    void Attack()
    {
        I.Enter(Stance.Engaged);
        I.Exit(Stance.AttackReady);
        My.animator.SetTrigger("Attack");
        Invoke("ResetAttack", My.stat.attackRate);
        My.Audio.PlayOneShot(attackSound);
    }
    void Stop() => My.body.velocity = Vector3.zero;
    void Assess(Stance flag, float range)
    {
        if (Vector3.Distance(transform.position, My.enemy.transform.position) < range) I.Enter(flag);
        else I.Exit(flag);
    }
    void MoveToPlayer()
    {
        Vector3 velocity = My.stat.speed * Time.deltaTime * (My.enemy.transform.position - transform.position).normalized;
        velocity.y = My.body.velocity.y;
        My.body.velocity = velocity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Weapon") && I.AmIn(Stance.Damageable) && My.enemy.IsAttacking())
        {
            My.stat.hitpoints -= My.enemy.Damage();
            My.Audio.PlayOneShot(getHitSound);
            I.Enter(Stance.Aggressive);
            if (I.AmIn(Stance.Alive))
            {
                if (My.stat.hitpoints <= 0)
                {
                    I.Exit(Stance.Alive);
                    My.animator.SetTrigger("die");
                    DropLoot();
                }
                else My.animator.SetTrigger("getHit");
            }
            I.Exit(Stance.Damageable);
            Debug.Log("Enemy has " + My.stat.hitpoints + " left!");
            Invoke("ResetIFrames", My.stat.iFrames);
        }
    }
    private void ResetIFrames() => I.Enter(Stance.Damageable);
    void ResetAttack()
    {
        I.Enter(Stance.AttackReady);
        I.Exit(Stance.Engaged);
    }
    void DropLoot()
    {
        int dropRate = Random.Range(0, 101);
        if (dropRate > 25) return;
        int index = Random.Range(0, drops.Length);
        Instantiate(drops[index], transform.position + Vector3.up, transform.rotation);
    }
}
