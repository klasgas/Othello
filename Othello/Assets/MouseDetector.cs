using UnityEngine;
using System.Collections;
using OthelloLogic;

public class MouseDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver () {
		if (Input.GetMouseButtonDown (0)) {

			int x = this.GetComponent<TileData>().x;
			int y = this.GetComponent<TileData>().y;

			DiscPlacer.TryMove(x, y);
		}
	}
}



