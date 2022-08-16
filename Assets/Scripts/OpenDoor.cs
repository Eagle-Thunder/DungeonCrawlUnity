using UnityEngine;
using TMPro;

public class OpenDoor : MonoBehaviour
{
    private float raiseTimer = 1;
    private const float speed = 5;
    protected bool isActive = false;
    protected PlayerController player;
    protected AudioSource doorAudio;
    public TextMeshProUGUI message;
    public AudioClip openDoor;
    public AudioClip listen;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        doorAudio = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (isActive && raiseTimer >= 0)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
            raiseTimer -= 1 * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && player.HasSilverKey())
        {
            message.text = "Press E to open door";
            message.gameObject.SetActive(true);
        } 
        else if (other.CompareTag("Player")) 
        {
            message.text = "You need the silver key to open this door";
            message.gameObject.SetActive(true);
        }
        if (other.CompareTag("Action") && player.HasSilverKey())
        {
            isActive = true;
            doorAudio.PlayOneShot(openDoor);
        }
        if (other.CompareTag("Listen") && listen)
        {
            doorAudio.PlayOneShot(listen, 4);
        }
    }
    private void OnTriggerExit(Collider other) => message.gameObject.SetActive(false);
}
