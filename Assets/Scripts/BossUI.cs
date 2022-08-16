using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{
    public TextMeshProUGUI bossHP;
    public TextMeshProUGUI winMessage;
    MobBrain boss;

    void Start() => boss = GameObject.Find("BossBug").GetComponent<MobBrain>();
    void Update()
    {
        bossHP.text = (boss.Health() > 0) ? new string('I', boss.Health()) : "";
        if(bossHP.text!="") Invoke("Win", 0.7f);
    }
    void Win() => winMessage.gameObject.SetActive(true);
}
