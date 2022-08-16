using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	List<string> targets;
	void Start()
	{
		targets = new List<string>
		{
			"Town",
			"BossArena",
			"Prison"
		};
	}
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			foreach (string target in targets)
				if (CompareTag(target)) SceneManager.LoadScene(sceneName: target);
	}
}
