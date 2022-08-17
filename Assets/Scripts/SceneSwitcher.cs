using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//portals need to be tagged in world with the Name of the scene they link to (like 'Scene1' in this example) and the strings updated here
public class SceneSwitcher : MonoBehaviour
{
 //   private void OnCollisionEnter(Collision collision)
	//{
	//	if (collision.gameObject.CompareTag("Player")) {
	//		if (CompareTag("Scene1")) SceneManager.LoadScene(sceneName: "Scene1");
	//		if (CompareTag("Scene2")) SceneManager.LoadScene(sceneName: "Scene2");
	//		if (CompareTag("Prison")) SceneManager.LoadScene(sceneName: "Prison");
	//	}
	//}
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			if (CompareTag("Town")) SceneManager.LoadScene(sceneName: "Town");
			if (CompareTag("BossArena")) SceneManager.LoadScene(sceneName: "BossArena");
			if (CompareTag("Prison")) SceneManager.LoadScene(sceneName: "Prison");
		}
	}
}
