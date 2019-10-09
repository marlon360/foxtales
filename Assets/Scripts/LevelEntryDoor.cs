using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEntryDoor : MonoBehaviour {

	public string SceneName;
	public Sprite KeySprite;

	public Light doorLight;
	private bool completed = false;

	bool isLoading = false;

	GameManager gm;

	void Awake(){
		gm = GameManager.GetInstance ();

		LevelData levelData = gm.getLevelData (SceneName);
		if (levelData != null) {
			completed = levelData.completed;
		}
			
	}

	public void addToGM(){
		LevelData thisLevel = new LevelData ();
		thisLevel.clicks = 0;
		thisLevel.completed = completed;
		thisLevel.keySprite = KeySprite;
		thisLevel.scenename = SceneName;
		GameManager.GetInstance ().addLevel (thisLevel);
	}

	public void startLevel(){
		if (!isLoading) {
			if (SceneName != "") {
				SceneChanger.loadScene (SceneName);
				isLoading = true;
			}
		}
	}

	// Use this for initialization
	void Start () {

		if (completed) {
			doorLight.color = Color.green;
		} else {
			doorLight.color = Color.red;
		}

	}



}
