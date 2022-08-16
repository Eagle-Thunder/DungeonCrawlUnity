using UnityEngine;

public class BugDamage : MonoBehaviour
{
    private BugBrain brain;
    public void Init(BugBrain Brain) => brain = Brain;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) brain.HitPlayer(other.gameObject);
    }
}
