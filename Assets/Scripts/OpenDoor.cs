using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenDoor : MonoBehaviour
{
    private float raiseTimer = 1;
    protected bool isActive = false;
    private float speed = 5;
    public TextMeshProUGUI message;
    protected PlayerController player;
    protected AudioSource doorAudio;
    public AudioClip openDoor;
    public AudioClip listen;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        doorAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && raiseTimer >= 0)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            raiseTimer -= 1 * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && player.hasSilverKey)
        {
            message.text = "Press E to open door";
            message.gameObject.SetActive(true);
        } 
        else if (other.CompareTag("Player")) 
        {
            message.text = "You need the silver key to open this door";
            message.gameObject.SetActive(true);
        }
        if (other.CompareTag("Action") && player.hasSilverKey)
        {
            isActive = true;
            doorAudio.PlayOneShot(openDoor);
        }
        if (other.CompareTag("Listen") && listen)
        {
            doorAudio.PlayOneShot(listen, 4);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        message.gameObject.SetActive(false);
    }
}
