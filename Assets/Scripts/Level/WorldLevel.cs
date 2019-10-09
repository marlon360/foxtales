using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldLevel : Level {

	public LevelEntryDoor[] levelEntries;


	public override void Awake(){
	
		base.Awake();

		if (!GameManager.GetInstance ().AreLevelSetUp ()) {
			foreach (LevelEntryDoor levelEntry in levelEntries) {
				levelEntry.addToGM ();
			}
            GameManager.GetInstance().restorLevelStatus();
			GameManager.GetInstance ().worldLevel = SceneManager.GetActiveScene ().buildIndex;
			GameManager.GetInstance ().SetUpKeysInUI ();
			UIManager.GetInstance ().ShowSmallMessage ("Finde alle Schlüssel!", 5);
		} else {
            GameManager.GetInstance().restorLevelStatus();
			GameManager.GetInstance ().SetUpKeysInUI ();
		}

	}

	public override void Start(){
		base.Start ();
	}

}
