using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour {

	private static SceneChanger instance;
	public GameObject loadingObject;
	public Image slider;

	void Start(){
		StartCoroutine (FadeOut (loadingObject, 1.5f));
	}

	private static SceneChanger GetInstance(){
		if (!instance) {
			instance = FindObjectOfType(typeof (SceneChanger)) as SceneChanger;
			if (!instance)
				Debug.LogError ("There needs to be one active SceneChanger script on a GameObject in your scene.");
		}

		return instance;
	}

	public static void loadScene(string SceneName){
		SceneChanger sc = GetInstance ();
		if (sc != null) {
			GetInstance ().loadSceneAsyn (SceneName);
		} else {
			SceneManager.LoadScene (SceneName);
		}
	}

	public static void loadScene(int SceneName){
		SceneChanger sc = GetInstance ();
		if (sc != null) {
			GetInstance ().loadSceneAsyn (SceneName);
		} else {
			SceneManager.LoadScene (SceneName);
		}
	}

	private void loadSceneAsyn(string SceneName){
		StartCoroutine (AsynchronousLoad (SceneName));
	}
	private void loadSceneAsyn(int SceneIndex){
		StartCoroutine (AsynchronousLoad (SceneIndex));
	}

	private IEnumerator FadeIn(GameObject group, float time){

		group.SetActive (true);
		RectTransform image = group.GetComponent<RectTransform> ();

		float elapsedTime = 0;
		image.localScale = new Vector3 (45, 45, 1);

		while (elapsedTime < time)
		{
			image.localScale = Vector3.Lerp(image.localScale, Vector3.one, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}


	}

	private IEnumerator slideDown(){
		
		float elapsedTime = 0;


		while (elapsedTime < 1) {
			slider.fillAmount = Mathf.Lerp(slider.fillAmount,0,elapsedTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}

	private IEnumerator FadeOut(GameObject group, float time){



		RectTransform image = group.GetComponent<RectTransform> ();
		image.localScale = Vector3.one;
		slider.fillAmount = 1;
		group.SetActive (true);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		StartCoroutine (slideDown ());
		yield return new WaitForSeconds (0.5f);

		float elapsedTime = 0;


		while (elapsedTime < time)
		{
			image.localScale = Vector3.Lerp(image.localScale, new Vector3 (45, 45, 1), (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		loadingObject.SetActive (false);

	}

	IEnumerator AsynchronousLoad (string scene)
	{
		yield return null;

		StartCoroutine (FadeIn (loadingObject,1));
		yield return new WaitForSeconds (0.6f);

		AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
		ao.allowSceneActivation = false;

		while (! ao.isDone)
		{
			// [0, 0.9] > [0, 1]
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			slider.fillAmount = Mathf.Lerp(slider.fillAmount,progress,Time.deltaTime*2f);
			// Loading completed
			if (ao.progress >= 0.9f)
			{
				if (slider.fillAmount > 0.9f) {
					ao.allowSceneActivation = true;
				}
			}

			yield return null;
		}
	}
	IEnumerator AsynchronousLoad (int scene)
	{
		yield return null;

		StartCoroutine (FadeIn (loadingObject,1));
		yield return new WaitForSeconds (0.6f);

		AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
		ao.allowSceneActivation = false;

		while (! ao.isDone)
		{
			// [0, 0.9] > [0, 1]
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			slider.fillAmount = progress;
			// Loading completed
			if (ao.progress >= 0.9f)
			{
				if (slider.fillAmount > 0.9f) {
					ao.allowSceneActivation = true;
				}
			}

			yield return null;
		}
	}
}
