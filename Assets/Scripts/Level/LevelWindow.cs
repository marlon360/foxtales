#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelWindow : EditorWindow
{

	Level level;
	InputHandler inputHandler;
	CameraOrbit camOrbit;
	Player player;
	ClickIndicator clickIndicator;
	UIManager uimanager;
	SceneChanger sChanger;

	Vector2 scrollPos;

	bool IsNotInLevel = false;


	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Level Debug")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		LevelWindow window = (LevelWindow)EditorWindow.GetWindow(typeof(LevelWindow));
		window.titleContent.text = "Level Debug";
		window.Show();
	}

	void Awake(){

		
		level = FindObjectOfType (typeof(Level)) as Level;
		inputHandler = FindObjectOfType (typeof(InputHandler)) as InputHandler;
		camOrbit = FindObjectOfType (typeof(CameraOrbit)) as CameraOrbit;
		player = FindObjectOfType (typeof(Player)) as Player;
		clickIndicator = FindObjectOfType (typeof(ClickIndicator)) as ClickIndicator;
		uimanager = FindObjectOfType (typeof(UIManager)) as UIManager;
		sChanger = FindObjectOfType (typeof(SceneChanger)) as SceneChanger;

		if (level != null) {
			IsNotInLevel = CheckElementsInLevel (level);
		}

	}

	void OnGUI()
	{
		if (!Application.isPlaying) {
			scrollPos = GUILayout.BeginScrollView (scrollPos);


			GUILayout.Label ("Level", EditorStyles.boldLabel);
			if (level != null) {
				if (GUILayout.Button ("Round Positions")) {
					level.roundAllPathElementsPosition ();
				}
				if (IsNotInLevel) {
					EditorGUILayout.HelpBox ("Some Elementes are not in Level GO!", MessageType.Warning);
					if (GUILayout.Button ("Fix it!")) {
						PutElementsInLevel (level);
					}
				}
				if (level.GetComponent<InputHandler> () != null) {
					EditorGUILayout.HelpBox ("Level has InputHandler!", MessageType.Warning);
					if (GUILayout.Button ("Fix it!")) {
						DestroyImmediate (level.GetComponent<InputHandler> ());
					}
				}
			} else {
				EditorGUILayout.HelpBox ("Level is missing!", MessageType.Error);
				if (GUILayout.Button ("Create Level")) {
					SetUpLevel ();
				}
			}
			
			if (inputHandler != null) {
				GUILayout.Label ("InputHandler is ready!", EditorStyles.boldLabel);
			} else {
				EditorGUILayout.HelpBox ("InputHandler is missing!", MessageType.Error);
				if (GUILayout.Button ("Create InputHandler")) {
					SetUpInputHandler ();
				}
			}


			if (camOrbit != null && camOrbit.transform.parent != null) {
				GUILayout.Label ("CameraOrbit is ready!", EditorStyles.boldLabel);
			} else {
				GUILayout.Label ("Camera is missing!", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox ("Camera is missing!", MessageType.Error);
				if (GUILayout.Button ("Create Camera")) {
					SetUpCamera ();
				}
			}

			if (player != null) {
				GUILayout.Label ("Player is ready!", EditorStyles.boldLabel);
				if (level != null && level.player == null) {
					EditorGUILayout.HelpBox ("Player not connected to Level!", MessageType.Error);
					if (GUILayout.Button ("Connect Player")) {
						level.player = FindObjectOfType (typeof(Player)) as Player;
					}
				}
				if (roundPosition (player.transform.position).x != player.transform.position.x || roundPosition (player.transform.position).z != player.transform.position.z) {
					EditorGUILayout.HelpBox ("Player Position is not rounded!", MessageType.Warning);
					if (GUILayout.Button ("Round Position")) {
						player.roundPosition ();
					}

				}
			} else {
				GUILayout.Label ("Player is missing!", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox ("Player is missing!", MessageType.Error);
				if (GUILayout.Button ("Create Player")) {
					SetUpPlayer (level);
				}
			}

			if (clickIndicator != null) {
				GUILayout.Label ("ClickIndicator is ready!", EditorStyles.boldLabel);
				if (level != null && level.clickIndicator == null) {
					EditorGUILayout.HelpBox ("ClickIndicator not connected to Level!", MessageType.Error);
					if (GUILayout.Button ("Connect ClickIndicator")) {
						level.clickIndicator = FindObjectOfType (typeof(ClickIndicator)) as ClickIndicator;
					}
				}
			} else {
				GUILayout.Label ("ClickIndicator is missing!", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox ("ClickIndicator is missing!", MessageType.Warning);
				if (GUILayout.Button ("Create ClickIndicator")) {
					SetUpClickIndicator ();
				}
			}

			if (uimanager != null) {
				GUILayout.Label ("UIManager is ready!", EditorStyles.boldLabel);
			} else {
				GUILayout.Label ("UIManager is missing!", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox ("UIManager is missing!", MessageType.Warning);
				if (GUILayout.Button ("Create UIManager")) {
					SetUpUIManager ();
				}
			}

			if (sChanger != null) {
				GUILayout.Label ("SceneTransition is ready!", EditorStyles.boldLabel);
			} else {
				GUILayout.Label ("SceneTransition is missing!", EditorStyles.boldLabel);
				EditorGUILayout.HelpBox ("SceneTransition is missing!", MessageType.Warning);
				if (GUILayout.Button ("Create SceneTransition")) {
					SetUpSceneChanger ();
				}
			}

			GUILayout.Space (50);

			if (GUILayout.Button ("Reload")) {
				Awake ();
			}
			if (GUILayout.Button ("Save Scene")) {
				EditorSceneManager.SaveScene (EditorSceneManager.GetActiveScene());
			}

			GUILayout.EndScrollView ();
		}
			
	}

	void OnHierarchyChange(){
		Awake ();
	}

	private static void SetUpCamera(){
		Camera mainCam = Camera.main;
		if (mainCam == null) {
			GameObject cam = PrefabUtility.InstantiatePrefab (Resources.Load ("Camera")) as GameObject;
			cam.name = "Camera";
		} else if (mainCam.gameObject == null || mainCam.gameObject.transform.parent == null || mainCam.gameObject.GetComponent<CameraOrbit> () == null) {
			if (mainCam.gameObject != null) {
				DestroyImmediate (mainCam.gameObject);
			}
			GameObject cam = PrefabUtility.InstantiatePrefab (Resources.Load ("Camera")) as GameObject;
			cam.name = "Camera";

		} else {
		
		}
	}

	private static void SetUpPlayer(){
		if (FindObjectOfType (typeof(Player)) == null) {
			GameObject player = PrefabUtility.InstantiatePrefab(Resources.Load ("Player")) as GameObject;
			player.name = "Player";
		}
	}
	private static void SetUpPlayer(Level level){
		if (FindObjectOfType (typeof(Player)) == null) {
			GameObject player = PrefabUtility.InstantiatePrefab(Resources.Load ("Player")) as GameObject;
			player.transform.position = new Vector3 (0, 1, 0);
			player.name = "Player";
			if (level != null) {
				level.player = player.GetComponent<Player> ();
			}
		}
	}


	private static void SetUpInputHandler(){
		if (FindObjectOfType (typeof(InputHandler)) == null) {
			GameObject player = PrefabUtility.InstantiatePrefab (Resources.Load ("InputHandler")) as GameObject;
			player.name = "InputHandler";
		}
	}

	private static void SetUpClickIndicator(){
		if (FindObjectOfType (typeof(ClickIndicator)) == null) {
			GameObject player = PrefabUtility.InstantiatePrefab (Resources.Load ("ClickIndicator")) as GameObject;
			player.name = "ClickIndicator";
		}
	}
	private static void SetUpClickIndicator(Level level){
		if (FindObjectOfType (typeof(ClickIndicator)) == null) {
			GameObject ind = PrefabUtility.InstantiatePrefab (Resources.Load ("ClickIndicator")) as GameObject;
			ind.name = "ClickIndicator";
			if (level != null) {
				level.clickIndicator = ind.GetComponent<ClickIndicator>();
			}
		}
	}

	private static void SetUpUIManager(){
		if (FindObjectOfType (typeof(UIManager)) == null) {
			GameObject uim = PrefabUtility.InstantiatePrefab (Resources.Load ("UI")) as GameObject;
			uim.name = "UI";
			GameObject es = PrefabUtility.InstantiatePrefab (Resources.Load ("EventSystem")) as GameObject;
			es.name = "EventSystem";
		}
	}

	private static void SetUpSceneChanger(){
		if (FindObjectOfType (typeof(SceneChanger)) == null) {
			GameObject sc = PrefabUtility.InstantiatePrefab (Resources.Load ("SceneTransition")) as GameObject;
			sc.name = "SceneTransition";
		}
	}

	private static void PutElementsInLevel(Level level){
		PathElement[] elemets = FindObjectsOfType (typeof(PathElement)) as PathElement[];
		foreach (PathElement el in elemets) {
			if (el.transform.parent == null) {
				el.transform.parent = level.transform;
			}
		}
	}
	private static bool CheckElementsInLevel(Level level){
		PathElement[] elemets = FindObjectsOfType (typeof(PathElement)) as PathElement[];
		foreach (PathElement el in elemets) {
			if (el.transform.parent == null) {
				return true;
			}
		}
		return false;
	}

	private static void SetUpLevel(){
		if (FindObjectOfType (typeof(Level)) == null) {
			GameObject level = new GameObject ("Level");
			level.AddComponent (typeof(KeyLevel));
			GameObject standardcube = PrefabUtility.InstantiatePrefab (Resources.Load ("PathElements/StandardCube")) as GameObject;
			standardcube.transform.parent = level.transform;
			standardcube.transform.position = new Vector3 (0, 0, 0);

			SetUpPlayer (level.GetComponent<Level>());
			SetUpCamera ();
			SetUpInputHandler ();
			SetUpClickIndicator (level.GetComponent<Level>());
			SetUpUIManager ();
			SetUpSceneChanger ();

		}
	}

	private static Vector3 roundPosition(Vector3 vec){
		return new Vector3 (Mathf.Round (vec.x), Mathf.Round (vec.y), Mathf.Round (vec.z));
	}


	[MenuItem ("GameObject/Create Empty Level")]
	private static void CreateEmptyLevel ()
	{
		SetUpLevel ();

	}
	[MenuItem ("GameObject/Create Standard Level (10x10)")]
	private static void CreateStandardLevel ()
	{

		int height = 10;
		int width = 10;

		SetUpLevel ();
		Level level = FindObjectOfType (typeof(Level)) as Level;


		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				GameObject standardcube = PrefabUtility.InstantiatePrefab (Resources.Load ("PathElements/StandardCube")) as GameObject;
				standardcube.transform.parent = level.transform;
				standardcube.transform.position = new Vector3 (i, 0, j);
			}
		}

	}


}
#endif