using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	public GameObject popupYouMustPass;

	void Awake() {
		Messenger.AddListener(GameEvent.PLAYER_MUST_PASS, ShowPopupYouMustPass);
	}

	public void ShowPopupYouMustPass()
	{
		var buttonScript = popupYouMustPass.GetComponent<OKButtonScript> ();
		buttonScript.ShowPopup ("Hej!");

	}

	public void ClosePopupYouMustPass()
	{
		popupYouMustPass.SetActive (false);
	}
	

	void OnDestroy() {
		Messenger.RemoveListener(GameEvent.PLAYER_MUST_PASS, ShowPopupYouMustPass);
	}
	
}
