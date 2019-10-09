using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour {

	public Image MainButton;
	public Color ButtonColorInGame;
	public Color ButtonColorInMenu;

	[Header("Message Panel")]
	public GameObject MessagePanel;
	bool IsMessagePanelActive = false;
	public Text MessageText;

	[Header("Small Message Panel")]
	public GameObject SmallMessagePanel;
	bool IsSmallMessagePanelActive = false;
	public Text SmallMessageText;

	[Header("Menu Panel")]
	public GameObject MenuPanel;
	bool IsMenuPanelActive = false;
	public GameObject KeyPanel;
	public Text clicks;

	[Header("Modal Panel")]
	public GameObject ModalPanel;
	bool IsModalPanelActive = false;
	public Text ModalText;
	public Button OkayButton;
	public Button CancelButton;

	[Header("Level Name Panel")]
	public GameObject LevelNamePanel;
	bool IsLevelNamePanelActive = false;
	public Text LevelName;

	[Header("End Panel")]
	public GameObject EndPanel;
	public Text clickText;

	[Space]
	public Material KeyOverlay;
	public Font font;

	void Awake(){
		MainButton.color = ButtonColorInGame;
	}


	private static UIManager instance;

	public static UIManager GetInstance () {
		if (!instance) {
			instance = FindObjectOfType(typeof (UIManager)) as UIManager;
			if (!instance)
				Debug.LogError ("There needs to be one active UIManager script on a GameObject in your scene.");
		}

		return instance;
	}


	private IEnumerator FadeIn(GameObject group, float time){

		group.SetActive (true);
		CanvasGroup canvasGroup = group.GetComponent<CanvasGroup>();
		canvasGroup.interactable = true;
		float elapsedTime = 0;
		canvasGroup.alpha = 0;

		while (elapsedTime < time)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}


	}
	private IEnumerator FadeOut(GameObject group, float time){

		CanvasGroup canvasGroup = group.GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		float elapsedTime = 0;
		canvasGroup.alpha = 1;

		while (elapsedTime < time)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			if (elapsedTime > time) {
				group.SetActive (false);
			}
			yield return new WaitForEndOfFrame();
		}



	}

	private IEnumerator FadeButtonIn(float time){


		float elapsedTime = 0;
		MainButton.color = ButtonColorInGame;

		while (elapsedTime < time)
		{
			MainButton.color = Color.Lerp(MainButton.color, ButtonColorInMenu, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator FadeButtonOut(float time){

		float elapsedTime = 0;
		MainButton.color = ButtonColorInMenu;

		while (elapsedTime < time)
		{
			MainButton.color = Color.Lerp(MainButton.color, ButtonColorInGame, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}


	public void ShowMessage(string Message){
		MessageText.text = Message;
		if (!IsMessagePanelActive) {
			IsMessagePanelActive = true;
			StartCoroutine (FadeIn (MessagePanel, 2));
			StartCoroutine (FadeButtonIn (2));
		}
	}

	public void HideMessage(){
		if (IsMessagePanelActive) {
			IsMessagePanelActive = false;
			StartCoroutine (FadeOut (MessagePanel, 2));
			StartCoroutine (FadeButtonOut (2));
		}
	}

	IEnumerator ShowSmallMessageForTime(string Message, float duration){
		SmallMessageText.text = Message;
		if (!IsSmallMessagePanelActive) {
			IsSmallMessagePanelActive = true;
			StartCoroutine (FadeIn (SmallMessagePanel, 2));
		}
		yield return new WaitForSeconds (duration);

		if (IsSmallMessagePanelActive) {
			IsSmallMessagePanelActive = false;
			StartCoroutine (FadeOut (SmallMessagePanel, 2));
		}
	}

	public void ShowSmallMessage(string Message, float duration){
		if (!IsMenuPanelActive) {
			StartCoroutine (ShowSmallMessageForTime (Message, duration));
		}
	}

	public void ShowSmallMessage(string Message){
		if (!IsMenuPanelActive) {
			SmallMessageText.text = Message;
			if (!IsSmallMessagePanelActive) {
				IsSmallMessagePanelActive = true;
				StopAllCoroutines ();
				StartCoroutine (FadeIn (SmallMessagePanel, 2));
			}
		}
	}
	public void HideSmallMessage(){
		if (IsSmallMessagePanelActive) {
			IsSmallMessagePanelActive = false;
			StartCoroutine (FadeOut (SmallMessagePanel, 2));
		}
	}

	IEnumerator ShowLevelNameForTime(string Name, float duration, float delay = 0f){
		yield return new WaitForSeconds (delay);
		ShowLevelName (Name);
		yield return new WaitForSeconds (duration);
		HideLevelName ();
	}
	public void ShowLevelName(string Name, float duration, float delay = 0f){
		StartCoroutine (ShowLevelNameForTime (Name, duration, delay));
	}

	public void ShowLevelName(string Name){
		LevelName.text = Name;
		if (!IsLevelNamePanelActive) {
			IsLevelNamePanelActive = true;
			StartCoroutine (FadeIn (LevelNamePanel, 2));
		}

	}
	public void HideLevelName(){
		if (IsLevelNamePanelActive) {
			IsLevelNamePanelActive = false;
			StartCoroutine (FadeOut (LevelNamePanel, 2));
		}
	}


	// Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
	public void ShowModal (string question, UnityAction yesEvent, UnityAction noEvent) {

		if (!IsModalPanelActive) {

			OkayButton.onClick.RemoveAllListeners ();
			OkayButton.onClick.AddListener (yesEvent);
			OkayButton.onClick.AddListener (CloseModal);

			CancelButton.onClick.RemoveAllListeners ();
			CancelButton.onClick.AddListener (noEvent);
			CancelButton.onClick.AddListener (CloseModal);


			ModalText.text = question;

			StartCoroutine (FadeIn (ModalPanel, 1));
			IsModalPanelActive = true;
		}

	}

	void CloseModal () {
		if (IsModalPanelActive) {
			StartCoroutine (FadeOut (ModalPanel, 1));
			StartCoroutine (FadeButtonOut (1));
			IsModalPanelActive = false;
		}
	}

	public void ClearKeys(){
		foreach(Transform child in KeyPanel.transform) {
			Destroy(child.gameObject);
		}
	}

	public void AddKey(LevelData level){

		float count = KeyPanel.GetComponent<RectTransform> ().childCount/2;
		float spaceBetweenKeys = 0.01f;

		GameObject NewKey = new GameObject (level.keySprite.name);
		GameObject KeyClick = new GameObject(level.scenename+"_clicks");

		NewKey.transform.SetParent (KeyPanel.transform);
		KeyClick.transform.SetParent (KeyPanel.transform);

		RectTransform rectTrans =  NewKey.AddComponent<RectTransform> ();
		rectTrans.anchorMin = new Vector2((count/10)+spaceBetweenKeys,0f);
		rectTrans.anchorMax = new Vector2((count/10)+0.1f,1f);
		rectTrans.offsetMax = new Vector2 (0, 0);
		rectTrans.offsetMin = new Vector2 (0, 0);

		RectTransform rectTransKeyClick =  KeyClick.AddComponent<RectTransform> ();
		rectTransKeyClick.anchorMin = new Vector2((count/10)+spaceBetweenKeys,0f);
		rectTransKeyClick.anchorMax = new Vector2((count/10)+0.1f,0.4f);
		rectTransKeyClick.offsetMax = new Vector2 (0, 0);
		rectTransKeyClick.offsetMin = new Vector2 (0, 0);

		Text keyClickText = KeyClick.AddComponent<Text> ();
		keyClickText.text = level.clicks + " Klicks";
		keyClickText.font = font;
		keyClickText.alignment = TextAnchor.UpperCenter;

		Image keyImage = NewKey.AddComponent<Image> ();
		keyImage.sprite = level.keySprite;
		keyImage.preserveAspect = true;
		if (!level.completed) {
			keyImage.material = KeyOverlay;
		}
	}



	public void ToggleMenu(){
		StopAllCoroutines ();
		if (IsMenuPanelActive) {
			IsMenuPanelActive = false;
			StartCoroutine (FadeOut(MenuPanel,1));
			StartCoroutine (FadeButtonOut (1));
		} else {
			IsMenuPanelActive = true;

			LevelNamePanel.SetActive (false);
			IsLevelNamePanelActive = false;
			SmallMessagePanel.SetActive (false);
			IsSmallMessagePanelActive = false;

			clicks.text = GameManager.GetInstance ().GetTotalClicks () + " Klicks insgesamt"; 

			StartCoroutine (FadeIn(MenuPanel,1));
			StartCoroutine (FadeButtonIn (1));
		}
	}

	public void ButtonClick(){

		if (IsMessagePanelActive) {
			HideMessage ();
		} else if (IsModalPanelActive) {
			CloseModal ();
		}else {
			ToggleMenu ();
		}

	}

	public void showEndPanel(int clicks){
		clickText.text = "Du hast "+clicks+" Klicks gebraucht!"; 
		StartCoroutine (FadeIn (EndPanel,2));
	}

	public void restartLevel(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void restartGame(){
		GameManager.GetInstance ().resetGame ();
	}


}
