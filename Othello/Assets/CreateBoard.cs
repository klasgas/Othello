using UnityEngine;
using System.Collections;

public class CreateBoard : MonoBehaviour {

	public GameObject tilePrefab;


	// Use this for initialization
	void Start () {

		float size = 1.1f;
		for (int y = 0; y < 8; y++) 
		{
			for (int x = 0; x < 8; x++) 
			{
				var tile = Instantiate(tilePrefab) as GameObject;
				tile.GetComponent<TileData>().x = x;
				tile.GetComponent<TileData>().y = y;

				tile.transform.position = new Vector3(x * size, 0, -y * size);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

