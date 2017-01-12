using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {

	public Vector2 tilePos;
	public int grade; // 100번대 몬스터

	///-----------monster일때만
	public int hp = 0;
	public int attackValue = 0;
	////---------------------
	public bool[] combine; // [dir]
	public bool combined;// [dir]
	public bool[] move;  // [dir]
	// Use this for initialization

	void Awake () {
		combine = new bool[4];
		combined = false;
		move= new bool[4];
		
		for (int i = 0; i < 4; ++i){
			combine[i] = false;
			move[i] = false;
		}
	}
	
	
	// Update is called once per frame
	void Update () {

	
	}
}
