using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

	private Animator _animator = null;
	bool keysFound = false;

	public SimplePathElement doorElement;
	public Light lightSource;



	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator> ();

		keysFound = GameManager.GetInstance().AllKeysFound ();

		if (keysFound) {
			lightSource.color = Color.green;
			UIManager.GetInstance ().ShowSmallMessage ("Du hast alle Schlüssel gefunden! Gehe durch das Tor!",5);
		}

	}
	

	public void TryToOpen(){
		if (keysFound) {
			_animator.SetBool ("toOpen", true);
			doorElement.SetActive (false);
		} else {
			UIManager.GetInstance ().ShowSmallMessage ("Du hast noch nicht alle Schlüssl gefunden!",3f);
		}
	}

	public void close(){
		doorElement.SetActive(true);
		_animator.SetBool ("toOpen", false);
		lightSource.color = Color.red;
	}
}