using UnityEngine;
using TMPro;

public class DropWall : MonoBehaviour
{
    public TextMeshProUGUI message;
    private Rigidbody wallRb;
    private AudioSource killWallAudio;
    public AudioClip crushSound;
    public AudioClip listen;
    private bool playedSound = false;

    void Start()
    {
        wallRb = GetComponent<Rigidbody>();
        killWallAudio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            message.text = "Press E to help";
            message.gameObject.SetActive(true);
        }
        if (other.CompareTag("Action") && !playedSound)
        {
            wallRb.isKinematic = false;
            playedSound = true;
            Invoke("PlaySound", 0.5f);
        }
        if (other.CompareTag("Listen")) killWallAudio.PlayOneShot(listen, 5);
    }
    private void OnTriggerExit(Collider other) => message.gameObject.SetActive(false);
    void PlaySound() => killWallAudio.PlayOneShot(crushSound, 2);
}
