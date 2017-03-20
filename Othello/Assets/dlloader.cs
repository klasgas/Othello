using UnityEngine;
using System.Collections;
using testproj;
using OthelloLogic;

public class dlloader : MonoBehaviour {

	public Board board;
//	public GameObject boardController;

	// Use this for initialization
	void Start () {
		Debug.Log(System.Environment.Version);
		var c = new testproj.MyClass ();
		Debug.Log (c.GetInfo ());

		board = new OthelloLogic.Board ();
		board.SetupInitialPosition ();

		DiscPlacer.SetPosition (board);
		DiscPlacer.TestAnimation ();
	}
	
	// Update is called once per frame
	void Update () {
		if (DiscPlacer.ComputerMoveFound ()) {
			DiscPlacer.ExecuteComputerMove();
		}
		//Debug.Log ("dlloader Update");
	}
}

