using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        hitPoints.text = "HP: " + player.hitPoints;
        damage.text = "DMG: " + player.damage;
        potions.text = "Potions: " + player.healthPotions;
        if (player.hasSilverKey)
        {
            silverKey.gameObject.SetActive(true);
        }
        if (player.hasGoldKey)
        {
            goldKey.gameObject.SetActive(true);
        }
        ShowControls();
    }
    void ShowControls()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (controls.IsActive())
            {
                controls.gameObject.SetActive(false);
            }
            else
            {
                controls.gameObject.SetActive(true);
            }
        }
    }
}
