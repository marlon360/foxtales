using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager {

	private Dictionary<string, LevelData> levels = new Dictionary<string, LevelData>();

	private static GameManager instance;

	public int worldLevel;

	public static GameManager GetInstance(){

		if(instance == null){
			instance = new GameManager();
		}

		return instance;
	}

	public void addLevel(LevelData level){
		levels.Add (level.scenename,level);
	}

	public void CompleteLevel(string levelName, int clicks){
		LevelData levelData;
		levels.TryGetValue (levelName, out levelData);
		if (levelData != null) {
			levelData.completed = true;
			levelData.clicks = clicks;
			saveLevelStatus ();
		}
	}


	public LevelData getLevelData(string levelName){
		LevelData levelData;
		levels.TryGetValue (levelName, out levelData);
		return levelData;
	}

	public Dictionary<string, LevelData> getLevels(){
		return levels;
	}

	public bool AreLevelSetUp(){
		return levels.Count != 0;
	}

	public bool AllKeysFound(){
		foreach (LevelData level in levels.Values) {
			if (!level.completed) {
				return false;
			}
		}
		return true;
	}


	public void SetUpKeysInUI(){

		UIManager.GetInstance ().ClearKeys ();
		foreach (LevelData level in levels.Values) {
			UIManager.GetInstance ().AddKey (level);
		}

	}

	public void resetGame(){
		levels.Clear ();
		PlayerPrefs.DeleteAll ();
		loadMainWorld ();
	}
	public void resetGameToTutorial(){
		levels.Clear ();
		PlayerPrefs.DeleteAll ();
		loadTutorialLevel ();
	}
		
	public void loadMainWorld(){
		SceneChanger.loadScene (worldLevel);
	}
	public void loadTutorialLevel() {
		SceneChanger.loadScene ("levelTutorial_mobil");
	}

	public void restartLevel(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public int GetTotalClicks() {
		int clicks = 0;
		foreach (LevelData level in levels.Values) {
			clicks += level.clicks;
		}
		return clicks;
	}

	public void saveLevelStatus() {
		foreach(LevelData level in levels.Values){
			PlayerPrefs.SetInt (level.scenename+"_completed", level.completed ? 1 : 0);
			if(PlayerPrefs.HasKey(level.scenename+"_clicks")){
                int clicks = PlayerPrefs.GetInt(level.scenename + "_clicks");
				if(clicks != 0 && clicks < level.clicks){
					level.clicks = clicks;
				}
			}

			PlayerPrefs.SetInt (level.scenename+"_clicks", level.clicks);
			
		}
	}

	public void restorLevelStatus(){
		foreach(LevelData level in levels.Values){
			if (PlayerPrefs.HasKey (level.scenename+"_completed")) {
				int completed = PlayerPrefs.GetInt (level.scenename+"_completed");
				level.completed = completed == 1;
				int clicks = PlayerPrefs.GetInt (level.scenename+"_clicks");
				level.clicks = clicks;

			}
		}
	}


}
[System.Serializable]
public class LevelData{

	public string scenename;
	public bool completed;
	public int clicks;
	public Sprite keySprite;


}
