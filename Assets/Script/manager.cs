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
	void UpdateTilesMoveDirByPosition(int x, int y){
		// 생성된 타일의 4방향 값을 셋팅한다.
		// 4 방향으로 move[dir] 값을 셋팅한다.
		tile nowTile= null;
		
		nowTile = GetTileInfoOnBoard(x,y);
		//
		for (int dir = 0; dir < 4; ++dir){
			bool tmpWall = true;
			tile compareTile = null;
			switch (dir){
				case 0://위
					if (y >= 1) {
						compareTile  = GetTileInfoOnBoard(x,y-1);
						tmpWall = false;
					}
					break;
				case 1://오른쪽
					if (x <= 2){
						compareTile  = GetTileInfoOnBoard(x+1,y);			
						tmpWall = false;
					} 
					break;
				case 2://아래
					if (y <= 2){
						compareTile  = GetTileInfoOnBoard(x,y+1);
						tmpWall = false;		
					} 	
					break;
				case 3://왼쪽
					if (x >= 1){
						compareTile  = GetTileInfoOnBoard(x-1,y);			
						tmpWall = false;
					} 
					break;
			}
			if(compareTile == null){
				if(!tmpWall){
					nowTile.move[dir] = true;
				}	
			}
			else{
				Debug.Log("dir(" + dir + ")(" + x +"," + y + ")" + nowTile.move[dir] + "compare(" + compareTile.tilePos.x + "," + compareTile.tilePos.y + ")" + compareTile.move[dir] );
				if(compareTile.move[dir]== false && nowTile.grade == compareTile.grade){
					nowTile.combine[dir] = true;
					nowTile.move[dir] = true;
				}
			}
		}
	}

	void UpdateTilesMoveDir(){
		GameObject[] weapons;
		GameObject background = GameObject.FindWithTag("Background");
		board CBoard;
		tile CTile;

		weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		CBoard = background.GetComponent<board>();
		foreach (GameObject weapon in weapons) {
			CTile = weapon.GetComponent<tile>();
			UpdateTilesMoveDirByPosition((int)CTile.tilePos.x , (int)CTile.tilePos.y);
		}
		
	}
	void UpdateTilesGradeOnBoard(){
		GameObject[] weapons;
		GameObject background = GameObject.FindWithTag("Background");
		tile CTile;
		board CBoard;

		weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		CBoard = background.GetComponent<board>();
		// for 너무 많이 돌지 말자. 있는거 셋팅 10이상으로  자리하고 빼자.
		foreach (GameObject weapon in weapons) {
			int tmpGrade = 10;
			CTile = weapon.GetComponent<tile>();
			tmpGrade =  CTile.grade + tmpGrade;
			CBoard.boardValue[ (int)CTile.tilePos.x, (int)CTile.tilePos.y] = tmpGrade;
			CBoard.tileOnBoard[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = CTile;
		}
		for (int i = 0; i < row; ++i){
			for (int j = 0; j < col; ++j){			
				if(CBoard.boardValue[i,j] <= 10){
					CBoard.boardValue[i,j] = 0;
					CBoard.tileOnBoard[i,j] = null;
				}
				else{
					CBoard.boardValue[i,j] = CBoard.boardValue[i,j] - 10;
				}
			}
		}
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
			tile CTile = weapon.GetComponent<tile>();

			if (CTile.move[dir]){
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
				CTile.tilePos = CTile.tilePos + Vtor;
				weapon.transform.localPosition = GridToWorld((int)CTile.tilePos.x , (int)CTile.tilePos.y);
				//UpdateTilesMoveDir((int)CTile.tilePos.x, (int)CTile.tilePos.y );
			}		
		}
	}

	void SetTileInfoToBoard(int x, int y, tile ctile){
		//특수 몬스터, 스킬에 사용하자.
		GameObject background = GameObject.FindWithTag("Background");
		background.GetComponent<board>().tileOnBoard[x,y] = ctile;
	}
	tile GetTileInfoOnBoard(int x, int y){
		tile weaponTile;
		GameObject background = GameObject.FindWithTag("Background");
		weaponTile = background.GetComponent<board>().tileOnBoard[x,y];
		return weaponTile;
	}
	bool CanSpawn(int x , int y){
		// 위치에 있는지 확인
		tile weaponTile;
		UpdateTilesGradeOnBoard();
		UpdateTilesMoveDir();
		weaponTile = GetTileInfoOnBoard(x,y);
		if(weaponTile == null){
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
				//UpdateTilesGradeOnBoard();
				UpdateTilesGradeOnBoard();
				UpdateTilesMoveDir();
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
