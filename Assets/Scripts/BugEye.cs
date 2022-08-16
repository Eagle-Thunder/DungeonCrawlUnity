using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugEye : MonoBehaviour {
	private BugBrain brain;
	public void Init(BugBrain Brain) => brain = Brain;
	public void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) brain.HuntStart(other.gameObject);
	}
	public void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("Player")) brain.HuntStop(other.gameObject);
	}
}
