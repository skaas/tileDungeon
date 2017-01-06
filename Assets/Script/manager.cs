using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class manager : MonoBehaviour {

	// 보드 가로,세로
	private int row = 4;
	private int col = 4;
	
	private int newTileCount = 0;
	// 한번에 많이 생성할 경우.
	public int newTileMaxCount = 1;
	

	// gameState
	bool waitingInput;
	bool waitingSpawn;

	// monster
	bool canSummonMobster;
	// gameOver
	public bool playerDead;
	public bool boardFilled;

	// pupup timing
	public bool levelup;
	public bool gameShop;

	// 이번 판에서 먹은 모든 자원
	int earnedGolds;
	int earnedKeys;
	
	// 몬스터가 맞아서 떨어진 자원 먹기전
	public int groundedGold = 0;
	public int groundedKeys = 0;

	public GameObject board; 
	public GameObject[] weaponTile; 
	public GameObject[] monster; 
	private Transform boardHolder;
	// Use this for initialization
	void Awake () { 
		// background 로딩
		boardHolder = new GameObject("baseObject").transform;
		boardHolder.position = new Vector3 (-2.82f,-4.34f,0f);

		GameObject instance = Instantiate (board, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
        instance.transform.SetParent (boardHolder);
		instance.transform.position = new Vector3 (-2.82f,-4.34f,0f);
		
		// 게임 시작
		GameStart();
	}
	
	// Update is called once per frame
	void Update () {
		if(waitingSpawn){
			Spawn();
		}
		if(waitingInput){
			if(Input.GetKeyUp(KeyCode.UpArrow)){
				Move(0);
				waitingSpawn = true;
				waitingInput = false;
			}    	
			if(Input.GetKeyUp(KeyCode.RightArrow)){
				Move(1);
				waitingSpawn = true;
				waitingInput = false;
			}	
			if(Input.GetKeyUp(KeyCode.DownArrow)){
				Move(2);
				waitingSpawn = true;
				waitingInput = false;
			}
			if(Input.GetKeyUp(KeyCode.LeftArrow)){
				Move(3);
				waitingSpawn = true;
				waitingInput = false;
			}
			UpdateTilesGradeOnBoard();
		}			
	}

	void GameStart(){
		
		// 시작 변수
		waitingInput = true;
		waitingSpawn = false;
		earnedGolds = 0;
		earnedKeys = 0;
		Spawn();
		//
	}
	// 타일 움직이기
	bool IsMove(GameObject weponTile, int dir){
		switch(dir) {
			case 0: 
				if ( weponTile.GetComponent<tile>().tilePos.y > 0) return true;
				break;
			case 1:
				if ( weponTile.GetComponent<tile>().tilePos.x < 3) return true;
				break;
			case 2:
				if ( weponTile.GetComponent<tile>().tilePos.y < 3) return true;
				break;
			case 3:
				if ( weponTile.GetComponent<tile>().tilePos.x > 0) return true;
				break;	
		}

		return false;
	}

	public void Move(int dir) {
		//0:up, 1:Right, 2:down, 3:Left
		
		GameObject[] weapons;
        weapons = GameObject.FindGameObjectsWithTag("WeaponTile");

		foreach (GameObject weapon in weapons) {
			Vector2 Vtor = Vector2.zero;
			if (IsMove(weapon, dir)){
				switch(dir) {
					case 0: 
						//weapon 의 local pos를 움직이고 weapon의 Tile 타일의 속성을 바꿔준다.
						Vtor = -Vector2.up;
						break;
					case 1:
						Vtor = Vector2.right;
						break;
					case 2:
						Vtor = Vector2.up;
						break;
					case 3:
						Vtor = -Vector2.right;
						break;	
				}	
			}
			//이거 tween으로 손가락이 간 만큼만 움직여야 해.threes한번 봐봐.
			//foreach로 한꺼번에 같이 움직이는게 될까?
			weapon.GetComponent<tile>().tilePos = weapon.GetComponent<tile>().tilePos + Vtor;
			weapon.transform.localPosition = GridToWorld((int)weapon.GetComponent<tile>().tilePos.x , (int)weapon.GetComponent<tile>().tilePos.y);	
		}
	}
	bool CheckMoveAble(){
		/*
		GameObject[] weapons;
		GameObject background = GameObject.FindWithTag("Background");
		Vector2 tilePos;
		weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		foreach (GameObject weapon in weapons) {
			int getGrade;
			tilePos =  weapon.GetComponent<tile>().tilePos;
			// dir =1 오른쪽
			if (tilePos.x < 3){
				getGrade = background.GetComponent<board>().boardValue[(int)tilePos.x +1, (int)tilePos.y];
				if(getGrade == 0){
					weapon.GetComponent<tile>().move[1] = true;
				}
				else if(weapon.GetComponent<tile>().grade == getGrade){
					weapon.GetComponent<tile>().move[1] = true;
					weapon.GetComponent<tile>().combine[1] = true;

					//합쳐지면 이하 타일이 모두 움직일 수 있다.
					

				}
			}
		}
		*/

		// 좌우 위아래를 체크해서 움직일 수 있는지 확인한다.
		// 최종 움직임과 상관없다. 위에 타일이 움직일 수 있으면 움직임이 가능함으로 바꿔줘야한다.
		return true;
	}
	void UpdateTilesGradeOnBoard(){
		GameObject[] weapons;
		GameObject background = GameObject.FindWithTag("Background");
		Vector2 tilePos;

		weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		
		// for 너무 많이 돌지 말자. 셋팅 10의 자리하고 빼자.
		foreach (GameObject weapon in weapons) {
			int tmpGrade = 10;
			tmpGrade = tmpGrade + weapon.GetComponent<tile>().grade;
			tilePos =  weapon.GetComponent<tile>().tilePos;
			background.GetComponent<board>().boardValue[ (int)tilePos.x, (int)tilePos.y] = tmpGrade;
		}
		for (int i = 0; i < row; ++i){
			for (int j = 0; j < col; ++j){		
				if(background.GetComponent<board>().boardValue[i,j] <= 10){
					background.GetComponent<board>().boardValue[i,j] = 0;
				}
				else{
					background.GetComponent<board>().boardValue[i,j] = background.GetComponent<board>().boardValue[i,j] - 10;
				}
				//Debug.Log("보드 업데이트 = [" + i + ","+ j +"]" + background.GetComponent<board>().boardValue[i,j]) ;
			}
		}
        
	}
	void SetTileInfoToBoard(int x, int y, int grade){
		//특수 몬스터, 스킬에 사용하자.
		GameObject background = GameObject.FindWithTag("Background");
		background.GetComponent<board>().boardValue[x,y] = grade;
	}
	int GetTileInfoOnBoard(int x, int y){
		int grade;
		GameObject background = GameObject.FindWithTag("Background");
		grade = background.GetComponent<board>().boardValue[x,y];
		return grade;
	}
	bool CanSpawn(int x , int y){
		// 위치에 있는지 확인
		GameObject[] weapons;
		int grade;
		
        weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		if(weapons.Length ==0){
			return true;
		}

		grade = GetTileInfoOnBoard(x,y);
		if(grade == 0){
			return true;
		}
		return false;
	}
	
	void Spawn(){
		int grade;
		Transform background = GameObject.FindWithTag("Background").transform;
		// 타일 랜덤하게 하나 생성 (적에 따라 다르게 생성해야 해.) todo
		GameObject summonTile = weaponTile[0];
		grade = 1;

		// 랜덤한 위치에 생성하기 있으면 waitingSpawn==false로 만들지 않아서 계속 되게 만든다.
		int x = Random.Range(0,row);
		int y = Random.Range(0,col);
		
		if(CanSpawn(x,y)){
			// 생성한 타일을 뿌려줌
			GameObject instance = Instantiate (summonTile, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
        	instance.transform.SetParent (background);
			instance.transform.localPosition = GridToWorld(x,y);
			instance.GetComponent<tile>().tilePos.x = (float) x;
			instance.GetComponent<tile>().tilePos.y = (float) y;
			instance.GetComponent<tile>().grade =grade;

			//혹시 새로운게 두 개 이상일 경우도 있으니. 테스트도 포함해서.
			newTileCount++;
			if(newTileMaxCount <= newTileCount){
				// 생성이 종료 되었음
				
				newTileCount = 0;
				waitingSpawn = false;
				waitingInput = true;
				UpdateTilesGradeOnBoard();
			}
		}
	}
	
	public static Vector3 GridToWorld(int x, int y) {
		return new Vector3(1.2f * x  + 0.6f, 5.7f - (1.2f *y) , 0f);
	}
	
	public static Vector2 WorldToGrid(float x, float y) {
		return new Vector2((x - 0.6f)/1.2f, (5.7f - y)/1.2f);
	}
}
