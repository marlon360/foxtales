using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class PathFinding {

	Level level;

	List<AStarObject> openList = new List<AStarObject>();
	List<AStarObject> closedList = new List<AStarObject>();

	List<Vector3> shortest = new List<Vector3>();


	public PathFinding(Level level){
		this.level = level;
	}
		

	Vector3 roundPosition(Vector3 position){
		return new Vector3(Mathf.Round(position.x),Mathf.Ceil(position.y),Mathf.Round(position.z));

	}

	public Vector3[] findPathTo(Vector3 start, Vector3 destination){

		int counter = 0;
		openList.Clear ();
		closedList.Clear ();

		AStarObject.setDestination (destination);

		AStarObject startElement = new AStarObject (null, start);
		openList.Add (startElement);


		do {
			
			counter++;

			AStarObject currentSquare = findLowestF();
			openList.Remove(currentSquare);
			closedList.Add(currentSquare);

			if(currentSquare.getPosition() == AStarObject.getDestination()){
				break;
			}

			List<AStarObject> adjacentSquares = findAdjacents(currentSquare);

			foreach (AStarObject nearAO in adjacentSquares) {
				if(containsAstar(nearAO,closedList)){
					continue;
				}
				if(!containsAstar(nearAO,openList)){
					openList.Add(nearAO);
				}else{
					//update G
				}
			}



		} while(openList.Count != 0);
			

		getShortestPath ();

		Vector3[] waypoints = shortest.ToArray ();
		System.Array.Reverse (waypoints);

		return waypoints;

	}

	//Liste aller benachbarten PathElements
	List<AStarObject> findAdjacents(AStarObject ao){

		List<AStarObject> adjacents = new List<AStarObject>();

		//Check if curretn Position is PathElement
		SimplePathElement currentPositionElement = null;
		level.GetAllElements().TryGetValue(ao.getPosition(), out currentPositionElement);

		Vector3[] directions = {
			Vector3.forward,
			Vector3.back,
			Vector3.left,
			Vector3.right
		};

		if (currentPositionElement != null){

			foreach (Vector3 direction in directions) {
				if (currentPositionElement.getBarrierByDirection(direction) == Barrier.Walkable) {
					AStarObject newAO = getAdjacent (ao, ao.getPosition(), direction);
					if (newAO != null) {
						adjacents.Add (newAO);
					}
				}
			}

		} else {

			foreach (Vector3 direction in directions) {
				AStarObject newAO = getAdjacent (ao, ao.getPosition(), direction);
				if (newAO != null) {
					adjacents.Add (newAO);
				}
			}

		}

		return adjacents;
	}

	AStarObject findLowestF(){
		AStarObject lowest = openList[0];

		foreach (AStarObject ao in openList) {
			if (lowest.greaterFThan(ao)) {
				lowest = ao;
			}
		}
		return lowest;

	}

	bool containsAstar(AStarObject element, List<AStarObject> list){
		foreach (AStarObject ao in list) {
			if (ao.getPosition() == element.getPosition()) {
				return true;
			}
		}
		return false;
	}

	AStarObject getAdjacent(AStarObject ao, Vector3 position, Vector3 direction){


		Vector3 walkPosition =  position + direction;
		Vector3 floorCubePosition = walkPosition + new Vector3 (0, -1, 0);

		SimplePathElement walkElement = null;
		SimplePathElement floorElement = null;

		level.GetAllElements().TryGetValue (walkPosition, out walkElement);
		level.GetAllElements().TryGetValue (floorCubePosition, out floorElement);


		//Normal Walk
		if (walkElement != null) {
			if (walkElement.bottomBarrier == Barrier.Walkable && walkElement.getBarrierByDirection (-direction) == Barrier.Walkable) {
				//Adjazenz Block existiert
				return new AStarObject(ao, position + direction);
			}
		} else {
			if (floorElement != null) {
				if (floorElement.topBarrier == Barrier.Walkable) {
					//Adjazenz Block existiert
					return new AStarObject(ao, position + direction);
				}
			}
		}

		//Stairs Up
		if (walkElement != null) {
			if (walkElement.getBarrierByDirection (-direction) == Barrier.Stairs) {
				SimplePathElement aboveStairEntry = null;
				Vector3 aboveStairEntryPosition = position + Vector3.up;
				level.GetAllElements().TryGetValue (aboveStairEntryPosition, out aboveStairEntry);
				if (aboveStairEntry == null) {
					//checke Treppen Ende
					return getAdjacent (ao, position + direction + Vector3.up, direction);
				}
			}
		}

		//Stairs Down
		if(walkElement == null){
			if(floorElement != null){
				if(floorElement.topBarrier == Barrier.Stairs){
					if (floorElement.getBarrierByDirection (direction) == Barrier.Stairs) {
						SimplePathElement aboveStairExit = null;
						Vector3 aboveStairExitPosition = walkPosition + direction;
						level.GetAllElements().TryGetValue (aboveStairExitPosition, out aboveStairExit);
						if (aboveStairExit == null) {
							//checke Treppen Start
							return getAdjacent (ao, position + direction + Vector3.down, direction);
						}
					}

				}
			}
		}

		return null;

	}


	bool checkWalkable(Vector3 position, Vector3 direction){


		Vector3 walkPosition =  position + direction;
		Vector3 floorCubePosition = walkPosition + new Vector3 (0, -1, 0);

		SimplePathElement walkElement = null;
		SimplePathElement floorElement = null;

		level.GetAllElements().TryGetValue (walkPosition, out walkElement);
		level.GetAllElements().TryGetValue (floorCubePosition, out floorElement);

		if (floorElement != null) {
			if (floorElement.topBarrier == Barrier.Walkable) {

				if (walkElement != null) {
					if (walkElement.getBarrierByDirection (-direction) == Barrier.Walkable) {
						return true;
					}
				} else {
					return true;
				}

			}
		}
		return false;

	}
		

	void getShortestPath(){

		shortest.Clear ();
		int G;

		AStarObject last = closedList [closedList.Count-1];
		G = last.getG();
		if (last.getPosition () == AStarObject.getDestination ()) {
			while (G != 0 && last.getParent () != null) {

				shortest.Add (last.getPosition());
				last = last.getParent ();
				G = last.getG ();
			}
		}

	}
		

}
