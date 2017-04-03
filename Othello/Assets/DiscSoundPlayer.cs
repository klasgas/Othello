using UnityEngine;
using System.Collections;

public class DiscSoundPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySound()
	{
		var audio = GetComponent<AudioSource> ();
		audio.Play ();
	}

}
