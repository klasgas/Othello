using UnityEngine;
using System.Collections;
using System.Text;
using OthelloLogic;

public class UIController : MonoBehaviour {

	public GameObject popupYouMustPass;
	OKButtonScript buttonScript;

	//private GameEvent currentPopup;  

	void Awake() {
		buttonScript = popupYouMustPass.GetComponent<OKButtonScript> ();
		Messenger.AddListener(GameEvent.PLAYER_MUST_PASS, ShowPopupYouMustPass);
		Messenger.AddListener(GameEvent.COMPUTER_MUST_PASS, ShowPopupComputerMustPass);
		Messenger.AddListener(GameEvent.GAME_OVER, ShowPopupGameOver);
	}

	private void ComputerPasses()
	{
		DiscPlacer._board.Pass();  ///funkade inte.... gick inte att göra sista draget efteråt. 
	}

	private void UserPasses()
	{
		DiscPlacer._board.Pass();
		DiscPlacer.StartMoveSearch ();
	}

	public void ShowPopupYouMustPass()
	{
		buttonScript.OnClose = UserPasses;
		StartCoroutine(WaitAndThenShowPopup("You must pass!"));
	}

	public void ShowPopupComputerMustPass()
	{
		buttonScript.OnClose = ComputerPasses;
		StartCoroutine(WaitAndThenShowPopup("I pass."));
	}

	public void ShowPopupGameOver()
	{
		buttonScript.OnClose = null;
		string message = GetGameOverMessage ();
		StartCoroutine(WaitAndThenShowPopup(message));
	}


	private string GetGameOverMessage()
	{
		PositionEvaluator positionEvaluator = new PositionEvaluator(DiscPlacer._board);
		int blackCount, whiteCount;
		positionEvaluator.GetBrickCount(out blackCount, out whiteCount);
		
		StringBuilder sb = new System.Text.StringBuilder();
		if (blackCount == whiteCount)
		{
			sb.Append("It's a draw! \n");
		}
		else if (blackCount > whiteCount)
		{
			sb.Append("Black won! \n");
		}
		else
		{
			sb.Append("White won! \n");
		}
		
		sb.Append("                                          \n");
		sb.Append("Black: " + blackCount.ToString() + "\n");
		sb.Append("White: " + whiteCount.ToString() + "\n");

		return sb.ToString ();
	}


/*	public void ClosePopupYouMustPass()
	{
		popupYouMustPass.SetActive (false);
	}*/


	IEnumerator WaitAndThenShowPopup(string text) {
		yield return new WaitForSeconds(1);  // to let animations finish playing.
		buttonScript.ShowPopup (text);
	}

	void OnDestroy() {
		Messenger.RemoveListener(GameEvent.PLAYER_MUST_PASS, ShowPopupYouMustPass);
		Messenger.RemoveListener(GameEvent.COMPUTER_MUST_PASS, ShowPopupComputerMustPass);
		Messenger.RemoveListener(GameEvent.GAME_OVER, ShowPopupGameOver);
	}
	
}
