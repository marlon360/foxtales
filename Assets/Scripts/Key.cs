using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Key : MonoBehaviour {

	KeyLevel level;

	void Start () {
		level = GetComponentInParent<KeyLevel> ();
	}

	public void setFound(){
		level.FoundKey ();
	}

}
