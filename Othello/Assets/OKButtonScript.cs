using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OKButtonScript : MonoBehaviour {

	public UnityEngine.UI.Text PopupText;

	public delegate void OnCloseCallback ();

	public OnCloseCallback OnClose;

	public void ShowPopup(string text)
	{
		PopupText.text = text;

		gameObject.SetActive (true);
	}
	
	public void ClosePopup()
	{
		gameObject.SetActive (false);

		if (OnClose != null) {
			OnClose ();
		}
	}
}
