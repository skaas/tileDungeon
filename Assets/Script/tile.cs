using UnityEngine;
using System.Collections;
using DigitalRuby.Tween;
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
	private int startMoveDir;
	public bool moving ;

	void Awake () {
		combine = new bool[4];
		combined = false;
		move= new bool[4];
		moving = false;
		for (int i = 0; i < 4; ++i){
			combine[i] = false;
			move[i] = false;
		}
	}
	
	
	// Update is called once per frame
	void Update () {
	}

	public void TileMove(Vector2 diffPosition, int movingDirection ){
		if(movingDirection == 0 || movingDirection == 2){
			Vector2 tmp = this.transform.localPosition;
			tmp.y = tmp.y + diffPosition.y;
			this.transform.localPosition =  tmp;
		}
		else if(movingDirection == 3|| movingDirection == 1){
			Vector2 tmp = this.transform.localPosition;
			tmp.x = tmp.x + diffPosition.x;
			this.transform.localPosition =  tmp;
		}
	}
	public void TileMoveNext(){
		int movingDirection = 0;
		Vector3 moveTo = Vector3.zero;
		Vector3 originPosition = new Vector3(1.2f * tilePos.x  + 0.3f, 6.6f - (1.2f *tilePos.y),0.0f);
		Vector2 nowPosition;
		
		nowPosition = originPosition - this.gameObject.transform.localPosition;
		if(nowPosition.x > 0){
			movingDirection = 1;
		}
		else if(nowPosition.x < 0) movingDirection = 3;
		else if(nowPosition.y > 0) movingDirection = 0;
		else if(nowPosition.y < 0) movingDirection = 2;

		if(movingDirection == 0){
			moveTo = new Vector3(1.2f * tilePos.x  + 0.3f, 6.6f - (1.2f *(tilePos.y-1)),0.0f);
		}
		else if(movingDirection == 1){
			moveTo = new Vector3(1.2f * tilePos.x +1  + 0.3f, 6.6f - (1.2f *(tilePos.y)),0.0f);
		}
		else if(movingDirection == 2){
			moveTo = new Vector3(1.2f * tilePos.x  + 0.3f, 6.6f - (1.2f *(tilePos.y+1)),0.0f);
		}
		else if(movingDirection == 3){
			moveTo = new Vector3(1.2f * tilePos.x -1  + 0.3f, 6.6f - (1.2f *(tilePos.y)),0.0f);
		}
		this.gameObject.Tween("tileAuto", this.gameObject.transform.localPosition, moveTo , 0.5f, TweenScaleFunctions.CubicEaseIn, (t) =>
            {
                // progress
                this.gameObject.transform.localPosition = t.CurrentValue;
            }, (t) =>
            {
            });
			

	}
	public void BackToOrigin(){
		Vector2 originPosition = new Vector2(1.2f * tilePos.x  + 0.3f, 6.6f - (1.2f *tilePos.y));
		this.transform.localPosition = originPosition;
		moving = false;
	}
}
