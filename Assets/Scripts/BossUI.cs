using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public TextMeshProUGUI bossHP;
    public TextMeshProUGUI winMessage;
    private MobBrain boss;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.Find("BossBug").GetComponent<MobBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.hp > 0)
        {
            bossHP.text = new string('I', boss.hp);
        }
        else
        {
            bossHP.text = "";
            Invoke("Win", 0.7f);
        }
    }
    void Win()
    {
        winMessage.gameObject.SetActive(true);
    }
}
