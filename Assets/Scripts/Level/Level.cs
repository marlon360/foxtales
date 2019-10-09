using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {

	//Der Spieler im Level
	public Player player;
	//Das Element, was einen Punkt oder ein X an der Stelle anzeigt, wo geklickt wurde
	public ClickIndicator clickIndicator;

	//Skript zur Pfadfindung
	protected PathFinding pathFinding;

	//Listen mit allen Pfad Elementen im Level
	Dictionary<Vector3, SimplePathElement> allElements = new Dictionary<Vector3, SimplePathElement> ();
	//Listen mit allen beweglichen Elementen im Level
	List<SimpleMovableElement> movableElements = new List<SimpleMovableElement>();

	//Level Reichweite (zur Berechnung des Bewegungsradius der beweglichen Elemente)
	Range3D levelRange = new Range3D();

	//Klickzähler, wie viele Klicks hat der Spieler gemacht
	public int clicks = 0;

	//Kamera Skript
	CameraOrbit MainCamera;

	public string LevelName;

	public virtual void Awake(){

		//Events anmelden
		InputHandler.hasClickedPathElement += hasClickedPathElement;
		InputHandler.isDraggingMovableElement += isDraggingMovableElement;
		InputHandler.isDragging += isDragging;
		InputHandler.endedDragging += endedDragging;

		player.reachedWaypoint += Player_reachedWaypoint;

		//Pfadfindung initialisieren für dieses Level
		pathFinding = new PathFinding (this);

		//Die Kamera mit Skript am Start festlegen (besser Performace)
		MainCamera = Camera.main.GetComponent<CameraOrbit>();


	}
		


	public virtual void Start () {
		//Alle Pfad Elemente dem Level hinzufügen
		updateAllElementsList ();
		//Die Level Reichweite aus allen Elemente erstellen (Beispiel: Das Element, welches ganz links ist bestimmt den minimalen X Punkt)
		levelRange.setRangeFromArray (allElements);
		//Wohin können die beweglichen Elemente geschoben werden (bis Level Reichweite, damit man nicht unendlich schieben kann)
		updateAllMoveRanges ();
		//Kamera folgt immer Spieler
		MainCamera.target = player.transform;

		if (LevelName != null && LevelName != "") {
			UIManager.GetInstance ().ShowLevelName (LevelName, 5, 1);
		}
	}


	//Alle Pfad Elemente dem Level hinzufügen
	void updateAllElementsList(){
		foreach (Transform cube in transform) { //Alle Elemente
			PathElement element = cube.GetComponent<PathElement> ();
			if (element != null) { //Wenn es ein PathElement ist
				element.setLevel (this); //Dem Element dieses Level zuordnen
				element.add (); //Diesem Level das Element hinzufügen
			}
		}
	}
		
	//Wohin können die beweglichen Elemente geschoben werden
	public void updateAllMoveRanges(){
		foreach (SimpleMovableElement element in movableElements) { //alle beweglichen Elemente
			element.updateMoveRange ();
		}
	}


	/*   EVENTS   */

	//Wenn auf ein PfadElement gelickt wurde
	void hasClickedPathElement(PathElement pathElement){
		//Mit Pfadfindung die Wegpunkte bestimmen
		Vector3[] waypoints = pathFinding.findPathTo (player.getPosition() ,pathElement.getPosition() + Vector3.up);

		//Klick Indikator anzeigen
		clickIndicator.clicked (pathElement.getPosition (),waypoints.Length != 0);
		//Wenn ein Weg gefunden wurde, Klickzähler erhöhen
		if (waypoints.Length != 0) {
			//Wegpunkte an Spieler übergeben
			player.setNewWaypoints (waypoints);
			clicks++;
		}
	}

	//Wenn ein Element bewegt wird
	void isDraggingMovableElement(SimpleMovableElement movableElement, Vector3 dragPosition){

	}

	//Wenn Spieler einen Wegpunkt abgelaufen ist
	void Player_reachedWaypoint (){
		//Alle Bewegungs Reichweiten aktualisieren, falls Spieler im Weg ist
		updateAllMoveRanges ();
	}

	//Wenn Element wieder losgelassen wird
	protected virtual void endedDragging (SimpleMovableElement obj){
		//Klicks hochzählen
		clicks++;
	}
		

	//Wenn gedraggt wird, aber kein bewegliches Element gewählt ist
	protected virtual void isDragging(float x, float y){
		//Kamera drehen, anhand von der Mouse/Touch Position
		MainCamera.CameraDisabled = false;
		MainCamera.pointer_x = x;
		MainCamera.pointer_y = y;
	}

	//Spieler Position aktualisieren
	public void setPlayerPosition(Vector3 position){
		player.setPosition (position);
	}



	public void roundAllPathElementsPosition(){
		
		gameObject.transform.position = new Vector3 (0, 0, 0);

		foreach (Transform cube in transform){
			if (cube.GetComponent<PathElement>() != null && cube.gameObject.activeSelf == true) {
				
				cube.GetComponent<PathElement>().roundPosition ();
				cube.GetComponent<PathElement> ().roundRotation ();

				SimpleMovableElement movableElement = cube.GetComponent<SimpleMovableElement> ();
				if (movableElement != null) {
					movableElement.turnModelToDirection ();
				}

			}
		}
	}

	public void addToAllElements(Vector3 pos, SimplePathElement element){
		if (!allElements.ContainsKey (pos)) {
			allElements.Add (pos, element);
		} else {
			//TODO check which ELement is stronger
		}
	}
	public void addToMovableElements(SimpleMovableElement element){
		movableElements.Add(element);
	}
	public void removeFromAllElements(Vector3 pos){
		if (allElements.ContainsKey(pos)){
			allElements.Remove (pos);
		}
	}
	public void removeFromMovableElements(SimpleMovableElement element){
		movableElements.Remove (element);
	}
	public void updateElementPosition(Vector3 oldPosition, Vector3 newPosition, SimplePathElement element){

		SimplePathElement oldPositionElement;
		allElements.TryGetValue (oldPosition, out oldPositionElement);
		if (oldPositionElement.Equals (element)) {
			allElements.Remove (oldPosition);
		}

		if (allElements.ContainsKey (newPosition)) {
			removeFromAllElements (newPosition);
		}
		allElements.Add (newPosition, element);

	}

	public Dictionary<Vector3, SimplePathElement> GetAllElements(){
		return allElements;
	}

	public Player GetPlayer(){
		return player;
	}
	public Range3D getRange(){
		return levelRange;
	}

	public void restartLevel(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
		


	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		foreach (Vector3 pos in allElements.Keys) {
			Gizmos.DrawSphere(pos, 0.1f);
		}

	}


	void OnDisable(){
		InputHandler.hasClickedPathElement -= hasClickedPathElement;
		InputHandler.isDraggingMovableElement -= isDraggingMovableElement;
		InputHandler.isDragging -= isDragging;
		InputHandler.endedDragging -= endedDragging;

		player.reachedWaypoint -= Player_reachedWaypoint;
	}
		

}
