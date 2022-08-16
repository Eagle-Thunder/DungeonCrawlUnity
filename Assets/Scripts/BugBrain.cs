using UnityEngine;

public enum State:int{idle,hunting,fighting,dieing,dead,}

public class BugBrain : MonoBehaviour {
    GameObject target;
    Rigidbody body;
    int damage, defense, hp, speed;
    State state;
    bool CoolDown = false;
    public GameObject HealingPotion, Sword, Bow, Armor;
    private Animator animator;
    void Start() {
        BugBrain brain = GetComponent<BugBrain>();
        BugEye eye = GetComponentInChildren<BugEye>();
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        eye.Init(brain);
        speed = 500;
        damage = 2;
        defense = 1;
        hp = 20;
        state = State.idle;
        CoolDown = false;
        target = gameObject;
    }
    void Update() {
		switch (state) {
            case State.idle:
                body.AddForce(Vector3.forward * speed, ForceMode.Impulse);
                break;
            case State.hunting:
                transform.LookAt(target.transform);
                body.AddForce(Vector3.forward * speed * Time.deltaTime);
                break;
            case State.fighting:
                transform.LookAt(target.transform);
                if (hp <= 0) {
                    CancelInvoke();
                    state = State.dieing;
				}
                break;
            case State.dieing:
                Invoke("DropLoot", 1.0f);
                state = State.dead;
                break;
            case State.dead:
                break;
        }
    }
	public void OnCollisionEnter(Collision collision) {
        PlayerController player = target.GetComponent<PlayerController>();
        if (collision.gameObject.CompareTag("PlayerSword") && state == State.fighting && !CoolDown)
        {
            hp -= (int)(player.Damage() - defense);
            CoolDown = true;
            Invoke("CoolDownReset", 0.5f);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("StabAttack");
            state = State.fighting;
        }
        else
        {
            transform.Rotate(Vector3.up, -90.0f, Space.Self);
        }
	}
    void CoolDownReset() => CoolDown = false;
	public void OnCollisionExit(Collision collision) {
		if (collision.gameObject == target) {
            animator.ResetTrigger("StabAttack");
            CancelInvoke();
            state = State.idle;
		}
	}
	public void HuntStart(GameObject who) {
        target = who;
        state = State.hunting;
	}
    public void HuntStop(GameObject who) {
        if (who == target) state = State.idle;
	}
    public void HitPlayer(GameObject who) {
        PlayerController player = who.GetComponent<PlayerController>();
        player.GetDamaged(damage);
	}
    void DropLoot() {
        Destroy(gameObject);
        float chance = Random.Range(0, 1);
        if (chance <= 0.25) Instantiate(HealingPotion, transform.position, transform.rotation);
        if (chance > 0.25 && chance <= 0.5) Instantiate(Sword, transform.position, transform.rotation);
        if (chance > 0.5 && chance <= 0.75) Instantiate(Bow, transform.position, transform.rotation);
        if (chance > 0.75) Instantiate(Armor, transform.position, transform.rotation);
    }
}
