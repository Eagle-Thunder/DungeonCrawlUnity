using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI hitPoints;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI potions;
    public TextMeshProUGUI silverKey;
    public TextMeshProUGUI goldKey;
    public TextMeshProUGUI controls;
    private PlayerController player;
    void Start() => player = GameObject.Find("Player").GetComponent<PlayerController>();
    void Update()
    {
        hitPoints.text = "HP: " + player.HitPoints();
        damage.text = "DMG: " + player.Damage();
        potions.text = "Potions: " + player.HealthPotions();
        if (player.HasSilverKey()) silverKey.gameObject.SetActive(true);
        if (player.HasGoldKey()) goldKey.gameObject.SetActive(true);
        ShowControls();
    }
    void ShowControls()
    {
        if (Input.GetKeyDown(KeyCode.R)) controls.gameObject.SetActive(!controls.IsActive());
    }
}
