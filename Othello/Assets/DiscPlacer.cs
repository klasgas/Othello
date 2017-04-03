using UnityEngine;
using System.Collections;
using System.Text;
using OthelloLogic;


public class DiscPlacer : MonoBehaviour
{
	public static Board _board;
	private static GameObject[,] _discs = new GameObject[8, 8];

	private static System.Threading.Thread _thread = null;
	private static IntPair? _computerMove = null;
	private static float startTimeForSearch;

	private static void TraceDiscValues()
	{
		var sb = new System.Text.StringBuilder();
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 8; x++)    
			{
				GameObject disc = _discs[x,y];
				Square.SquareValue discValue = GetDiscValue(disc);

				string s = ". ";
				if(discValue == Square.SquareValue.Black)
				{
					s = "# ";
				}
				else if(discValue == Square.SquareValue.White)
				{
					s = "O ";
				}

				sb.Append(s);
				//Debug.Write(square.ToString());
			}
			sb.AppendLine("");
		}

		Debug.Log (sb.ToString ());
	}

	public static bool MoveIsLegal(int x, int y)
	{
		if (_board._currentPlayer == Square.SquareValue.Black) {
			return _board.MoveIsLegal (x, y);
		} else {
			Debug.Log ("vänta på din tur");
			return false;
		}
	}

	public static void TryMove(int x, int y)
	{
		Debug.Log ("TRYMove");
		if (MoveIsLegal (x, y)) {
			Debug.Log ("move is legal");
			Debug.Log (string.Format ("making move x={0}, y={1}", x, y));
			_board.MakeMove(x, y);
			//var move = OthelloLogic.MoveSearcher.FindMove(_board);
			//_board.MakeMove(move._x, move._y);
			AddAndFlipDiscsAccordingToBoard();
			Debug.Log(_board.TraceBoard ("after move"));
			TraceDiscValues();

			//StartCoroutine(WaitAndThenStartMoveSearch);
			StartMoveSearch();
		}

	}

	IEnumerator WaitAndThenStartMoveSearch() {
		yield return new WaitForSeconds(1);  // to let animations finish playing.
		StartMoveSearch();
	}


	private static void StartMoveSearch()
	{
		_thread = new System.Threading.Thread(Run);
		_thread.Start ();
		startTimeForSearch = Time.time;
	}

	public static bool TimeSinceSearchStartIsMoreThanASecond()
	{
		return (Time.time - startTimeForSearch) > 1; 
	}

	public static bool ComputerMoveFound()
	{
		return _computerMove.HasValue;
	}

	public static void ExecuteComputerMove()
	{
		Debug.Log ("ExecuteComputerMove");
		int x = _computerMove.Value._x;
		int y = _computerMove.Value._y;
		_computerMove = null;

		if (!_board.CurrentPlayerMustPass()) {
			_board.MakeMove (x, y);
			AddAndFlipDiscsAccordingToBoard();
			//if(_board.CurrentPlayerMustPass())
			{
				//Messenger.Broadcast(GameEvent.PLAYER_MUST_PASS);
			}
		}
		{
			// gui: computer must pass.
			//Messenger.Broadcast(GameEvent.PLAYER_MUST_PASS);
		}

	}

	private static void Run()
	{
		//Debug.Log ("RUN");
		_computerMove = null;
		_computerMove = MoveSearcher.FindMove(new Board(_board));
		//Debug.Log ("_computerMove FOUND");
	}

	// Update is called once per frame

	public static void AddAndFlipDiscsAccordingToBoard()
	{
		//Messenger.Broadcast(GameEvent.PLAYER_MUST_PASS);
		for(int y = 0; y < 8; y++)
		{
			for(int x = 0; x < 8; x++)
			{
				Square.SquareValue value = _board._squares[x][y].GetValue();
				if(value != Square.SquareValue.Empty)
				{
					GameObject disc = _discs[x, y];

					Square.SquareValue discValue = GetDiscValue(disc);

					if(x == 0 && y == 0) 
					{
						Debug.Log (string.Format("discValue={0}", discValue));
					}

					if(discValue == Square.SquareValue.Empty)
					{
						PlaceDisc(x, y, value);
					}
					else
					{
						if(value != discValue)
						{
							RotateDisc(disc, true);
						}
					}
				}
			}
		}
	}

	private static Square.SquareValue GetDiscValue(GameObject disc)
	{
		if (disc == null) {
			Debug.Log ("disc is null");
			return Square.SquareValue.Empty;
		} else {
			return disc.GetComponent<DiscValue>().value;
		}
	}

	private static void RotateDisc(GameObject disc, bool animate)
	{
		var animator = disc.GetComponent<Animator> ();

		//disc.transform.Rotate(new Vector3(180, 0f, 0), Space.Self);
		var discValueComponent = disc.GetComponent<DiscValue> ();

		if (discValueComponent.value == Square.SquareValue.Black) {
			discValueComponent.value = Square.SquareValue.White;
			if(animate)
			{
				animator.SetTrigger ("FlipToWhite");
			}
			else
			{
				animator.Play ("IdleWhite");
			}

		} else {
			discValueComponent.value = Square.SquareValue.Black;
			if(animate)
			{
				animator.SetTrigger ("FlipToBlack");
			}
			else
			{
				animator.Play ("IdleBlack");
			}
		}
		
	}

	public static void PlaceDisc(int x, int y, Square.SquareValue value)
	{

		if (value != Square.SquareValue.Empty) {
			GameObject disc = Instantiate (Resources.Load ("Disc")) as GameObject; 
			disc.GetComponent<DiscValue> ().value = Square.SquareValue.Black; // disc prefab has black side up

//			disc.transform.position = new Vector3 ((x * 1.1f), 0.6f, (-y * 1.1f));
			disc.GetComponent<DiscValue> ().value = Square.SquareValue.Black; // disc prefab has black side up
			
			GameObject discParent = new GameObject("discParent");
			
			disc.transform.parent = discParent.transform;
			discParent.transform.position = new Vector3 ((x * 1.1f), 0.6f, (-y * 1.1f));

			
			if (value == Square.SquareValue.White) {
		//		Debug.Log (string.Format ("ROTATING NEW DISC TO WHITE"));
				RotateDisc (disc, false);
			}

			_discs [x, y] = disc;
		}
	}

	public static void TestAnimation()
	{
		int x = 0;
		int y = 0;

		GameObject disc = Instantiate (Resources.Load ("Disc")) as GameObject; 
		disc.GetComponent<DiscValue> ().value = Square.SquareValue.Black; // disc prefab has black side up

		GameObject discParent = new GameObject("discParent");

		disc.transform.parent = discParent.transform;
		discParent.transform.position = new Vector3 ((x * 1.1f), 0.6f, (-y * 1.1f));


		var animator = disc.GetComponent<Animator> ();
		animator.SetTrigger ("FlipToWhite");
	}


	public static void SetPosition(Board board)
	{
		if (_board == null) {
			_board = board;
		}
		

		for(int y = 0; y < 8; y++)
		{
			for(int x = 0; x < 8; x++)
			{
				Square.SquareValue value = _board._squares[x][y].GetValue();;

				if(value != Square.SquareValue.Empty)
				{
					DiscPlacer.PlaceDisc(x, y, value);
				}
			}
		}

	}
}