using UnityEngine;
using System.Collections;



public class Player : MonoBehaviour {

	public WorldObject SelectedObject{ get; set; }

	public HUD hud;

	public string username;
	public bool human;  //Didn't work previously, might have to get it to work so HUD won't draw for AI players

	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren< HUD > ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
