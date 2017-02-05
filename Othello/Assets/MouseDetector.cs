using UnityEngine;
using System.Collections;
using OthelloLogic;

public class MouseDetector : MonoBehaviour {
	public GameObject discPrefab;
	private static bool fjong = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver () {
		if (Input.GetMouseButtonDown (0)) {
			//Debug.Log ("Mouse click");

			int x = this.GetComponent<TileData>().x;
			int y = this.GetComponent<TileData>().y;

			//var positionHander = this.GetComponent<PositionHandler>();
			//PositionHandler.Ingenting();
			DiscPlacer.TryMove(x, y);
			//DiscPlacer.PlaceDisc(x, y, OthelloLogic.Square.SquareValue.Black);


			Debug.Log (fjong.ToString ());
			if(fjong)
			{
				//Debug.Log ("fjong");

			}
			fjong = !fjong;

		}
	}
}



