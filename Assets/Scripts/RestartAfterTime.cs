using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartAfterTime : MonoBehaviour {

	private static float maxTime = 40f;
	public static float timer;
	string text = "";

	bool loadingScene = false;
	bool firstClick = false;

	// Use this for initialization
	void Start () {
		timer = maxTime;
		if (SceneManager.GetActiveScene ().name != "levelTutorial_mobil") {
			firstClick = true;
		}
	}

	public static void resetTimer(){
		timer = maxTime;
	}
	
	// Update is called once per frame
	void Update () {

		if (firstClick) {
			timer -= Time.deltaTime;

			if (Input.anyKey) {
				resetTimer ();
			}
			if (timer < 5) {
				text = string.Format ("Restart in {0:0.} seconds! Touch the screen!", timer);
			} else {
				text = "";
			}
			if (timer < 0) {
				if (!loadingScene) {
					GameManager.GetInstance ().resetGameToTutorial ();
					loadingScene = true;
				}

			}
		} else {
			if (Input.anyKey) {
				firstClick = true;
			}
		}

	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 5, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperCenter;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (1f, 1f, 1f, 1.0f);
		GUI.Label(rect, text, style);
	}

}
