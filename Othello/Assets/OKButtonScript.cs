using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OKButtonScript : MonoBehaviour {

	public void ShowPopup(string text)
	{
		var button = gameObject.GetComponent<UnityEngine.UI.Button> ();
		if (button == null) {
			Debug.Log ("button är null");
		}
		var buttonText = button.GetComponent<UnityEngine.UI.Text>();

		buttonText.text = text;

		gameObject.SetActive (true);
	}
	
	public void ClosePopup()
	{
		gameObject.SetActive (false);
	}
}
