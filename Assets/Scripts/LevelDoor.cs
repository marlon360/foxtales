using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelDoor : MonoBehaviour {

	public bool isExit;

	string SceneName;
	GameManager gm;

	KeyLevel level;

	public string MainWorldScene;

	bool isLoadingLevel = false;

	void Awake(){
		gm = GameManager.GetInstance ();
		SceneName = SceneManager.GetActiveScene ().name;
		level = GetComponentInParent<KeyLevel> ();

	}

	public void setKey(){
		if(level != null && level.isKeyFound()){
			gm.CompleteLevel (SceneName,level.clicks);
		}
	}

	public void startLevel(){
		if (isExit) {
			if (level != null) {
				if (!level.isKeyFound ()) {
					UIManager.GetInstance ().ShowModal ("Du hast den Schlüssel noch nicht gefunden! Möchtest du wirklich zurück zur Hauptwelt?", new UnityAction (loadMainWorld), new UnityAction (() => {
					}));
				} else {
					setKey ();
					loadMainWorld ();
				}
			} else {
				loadMainWorld ();
			}
		} else {
			if (level != null) {
				UIManager.GetInstance ().ShowModal ("Das ist der Eingang! Wenn du hier durch gehst, geht der Schlüssel verloren! Möchtest du wirklich zurück zur Hauptwelt?", new UnityAction (loadMainWorld), new UnityAction (() => {
				}));
			} else {
				loadMainWorld ();
			}
		}
	}

	public void loadMainWorld(){
		if (!isLoadingLevel) {
			SceneChanger.loadScene (MainWorldScene);
			isLoadingLevel = true;
		}
	}
}
