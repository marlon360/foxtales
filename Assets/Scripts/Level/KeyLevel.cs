using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLevel : Level {

	private bool keyFound;

	public override void Awake(){

		base.Awake ();
		GameManager.GetInstance().SetUpKeysInUI ();
	}

	public void FoundKey(){
		keyFound = true;
	}

	public bool isKeyFound(){
		return keyFound;
	}

}
