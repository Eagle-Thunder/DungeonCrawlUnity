using UnityEngine;

public class OpenEndDoor : OpenDoor
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && player.HasGoldKey())
        {
            message.text = "Press E to open door";
            message.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Player"))
        {
            message.text = "You need the gold key to open this door";
            message.gameObject.SetActive(true);
        }
        if (other.CompareTag("Action") && player.HasGoldKey())
        {
            isActive = true;
            doorAudio.PlayOneShot(openDoor);
        }
    }
}
