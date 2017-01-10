using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {

	public Vector2 tilePos;
	public int grade;

	public bool[] combine; // [dir]
	public bool combined;// [dir]
	public bool[] move;  // [dir]
	public int[] gradeAround;
	// Use this for initialization

	

	void Awake () {
		combine = new bool[4];
		combined = false;
		move= new bool[4];
		gradeAround = new int[4];
		
		for (int i = 0; i < 4; ++i){
			combine[i] = false;
			move[i] = false;
		}
	}
	
	
	// Update is called once per frame
	void Update () {

	
	}
}
