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
	private bool[] canMove = new bool[4];
	

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
		if(boardFilled){
			Debug.Log("게임 종료 스페이스를 누르면 게임을 재식작합니다.");		
		}
		if(waitingSpawn){
			Spawn();
			IsGameOver();
		}
		if(waitingInput){
			if(Input.GetKeyUp(KeyCode.UpArrow)){
				if(canMove[0]){
					Move(0);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
				
			}    	
			if(Input.GetKeyUp(KeyCode.RightArrow)){
				if(canMove[1]){
					Move(1);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
			}	
			if(Input.GetKeyUp(KeyCode.DownArrow)){
				if(canMove[2]){
					Move(2);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
			}
			if(Input.GetKeyUp(KeyCode.LeftArrow)){
				if(canMove[3]){
					Move(3);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
			}
			if(Input.GetKeyDown(KeyCode.Space)){
				boardFilled = false;
				GameStart();
			}
			
		}			
	}

	void GameStart(){
		// 시작 변수
		waitingInput = true;
		waitingSpawn = false;
		earnedGolds = 0;
		earnedKeys = 0;
		CanMoveInit();
		ClearBoard();
		
		// 생성
		Spawn();
		//
	}
	void CanMoveInit(){
		canMove[0] = false;
		canMove[1] = false;
		canMove[2] = false;
		canMove[3] = false;
	}
	void ClearBoard(){
		GameObject[] weapons  = GameObject.FindGameObjectsWithTag("WeaponTile");
		GameObject background = GameObject.FindWithTag("Background");
		board CBoard = background.GetComponent<board>();

		// 타일 삭제
		foreach (GameObject weapon in weapons) {
			Destroy(weapon);
		}
		// 보드 초기화

		for (int i = 0; i < 4; ++i){
            for (int j = 0; j < 4; ++j){
                CBoard.boardValue[i,j] = 0;   
				CBoard.tileOnBoard[i,j] = null;
				CBoard.upgrade[i,j] = false;
            }
        }
	}
	void TileMoveInit(tile CTile){
		for(int dir = 0; dir < 4 ; ++dir){
				CTile.move[dir] = false;
				CTile.combine[dir] = false;
			}	
	}
	void UpdateTilesMoveDir(){
		GameObject background = GameObject.FindWithTag("Background");
		GameObject[] weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		board CBoard = background.GetComponent<board>();;
		tile CTile;

		CanMoveInit();
		
		foreach (GameObject weapon in weapons) {	
			CTile = weapon.GetComponent<tile>();
			TileMoveInit(CTile);
		}
		
		for (int i = 0; i < col; ++i){
			for (int j = 0; j < row; ++j){
				// dir - 0
				//CBoard.tileOnBoard[i,0],CBoard.tileOnBoard[i,1],CBoard.tileOnBoard[i,2],CBoard.tileOnBoard[i,3]
				//CBoard.tileOnBoard[i,j].move[0]
				// dir - 1
				// CBoard.tileOnBoard[3,i],CBoard.tileOnBoard[2,i],CBoard.tileOnBoard[1,i],CBoard.tileOnBoard[0,i]
				// CBoard.tileOnBoard[3-j,i].move[1]
				// dir - 2
				// CBoard.tileOnBoard[i,3],CBoard.tileOnBoard[i,2],CBoard.tileOnBoard[i,1],CBoard.tileOnBoard[i,0]
				// CBoard.tileOnBoard[i,3-j].mobe[2]
				// dir - 3
				// CBoard.tileOnBoard[0,i],CBoard.tileOnBoard[1,i],CBoard.tileOnBoard[2,i],CBoard.tileOnBoard[3,i]
				// CBoard.tileOnBoard[j,i].move[3] = true;
				if(CBoard.tileOnBoard[i,0] == null){
					if(CBoard.tileOnBoard[i,j] != null && j >=1 ) CBoard.tileOnBoard[i,j].move[0] = true;
				}
				else{
					if(CBoard.tileOnBoard[i,1] == null){
						if(CBoard.tileOnBoard[i,j] != null  && j >= 2) CBoard.tileOnBoard[i,j].move[0] = true;
					}
					else if( CBoard.tileOnBoard[i,0].grade == CBoard.tileOnBoard[i,1].grade ){
						if(CBoard.tileOnBoard[i,j] != null  && j >=1) {
							CBoard.tileOnBoard[i,j].move[0] = true;
							CBoard.tileOnBoard[i,1].combine[0] = true;
						}
					}
					else{
						if(CBoard.tileOnBoard[i,2] == null){
							if(CBoard.tileOnBoard[i,j] != null  && j >=3) CBoard.tileOnBoard[i,j].move[0] = true;
						}
						else if(CBoard.tileOnBoard[i,1].grade == CBoard.tileOnBoard[i,2].grade ){
							if(CBoard.tileOnBoard[i,j] != null  && j >= 2){
								CBoard.tileOnBoard[i,j].move[0] = true;
								CBoard.tileOnBoard[i,2].combine[0] = true;
							} 
						}
						else{
							if(CBoard.tileOnBoard[i,3]!= null){
								if(CBoard.tileOnBoard[i,2].grade == CBoard.tileOnBoard[i,3].grade){
									if(CBoard.tileOnBoard[i,j]  != null && j >=3){
										CBoard.tileOnBoard[i,j].move[0] = true;
										CBoard.tileOnBoard[i,3].combine[0] = true;
									} 
								}
							}
						}
					}
				}
				// dir - 1
				if(CBoard.tileOnBoard[3,i] == null){
					if(CBoard.tileOnBoard[3-j,i]!= null && j >=1 ) CBoard.tileOnBoard[3-j,i].move[1] = true;
				}
				else{
					if(CBoard.tileOnBoard[2,i] == null){
						if(CBoard.tileOnBoard[3-j,i]!= null && j >= 2) CBoard.tileOnBoard[3-j,i].move[1] = true;
					}
					else if( CBoard.tileOnBoard[3,i].grade == CBoard.tileOnBoard[2,i].grade ){
						if(CBoard.tileOnBoard[3-j,i]!= null && j >=1) {
							CBoard.tileOnBoard[3-j,i].move[1] = true;
							CBoard.tileOnBoard[2,i].combine[1] = true;
						}
					}
					else{
						if(CBoard.tileOnBoard[1,i] == null){
							if(CBoard.tileOnBoard[3-j,i]!= null && j >=3) CBoard.tileOnBoard[3-j,i].move[1] = true;
						}
						else if(CBoard.tileOnBoard[1,i].grade == CBoard.tileOnBoard[2,i].grade ){
							if(CBoard.tileOnBoard[3-j,i]!= null && j >= 2){
								CBoard.tileOnBoard[3-j,i].move[1] = true;
								CBoard.tileOnBoard[1,i].combine[1] = true;
							} 
						}
						else{
							if(CBoard.tileOnBoard[0,i]!= null){
								if(CBoard.tileOnBoard[0,i].grade == CBoard.tileOnBoard[1,i].grade){
									if(CBoard.tileOnBoard[3-j,i]!= null && j >=3){
										CBoard.tileOnBoard[3-j,i].move[1] = true;
										CBoard.tileOnBoard[0,i].combine[1] = true;		
									} 
								}
							}
						}
					}
				}
				// dir - 2
				if(CBoard.tileOnBoard[i,3] == null){
					if(CBoard.tileOnBoard[i,3-j]!= null && j >=1 ) CBoard.tileOnBoard[i,3-j].move[2] = true;
				}
				else{
					if(CBoard.tileOnBoard[i,2] == null){
						if(CBoard.tileOnBoard[i,3-j]!= null && j >= 2) CBoard.tileOnBoard[i,3-j].move[2] = true;
					}
					else if( CBoard.tileOnBoard[i,2].grade == CBoard.tileOnBoard[i,3].grade ){
						if(CBoard.tileOnBoard[i,3-j]!= null && j >=1){
							CBoard.tileOnBoard[i,3-j].move[2] = true;
							CBoard.tileOnBoard[i,2].combine[2] = true;
						} 
					}
					else{
						if(CBoard.tileOnBoard[i,1] == null){
							if(CBoard.tileOnBoard[i,3-j]!= null && j >=3) CBoard.tileOnBoard[i,3-j].move[2] = true;
						}
						else if(CBoard.tileOnBoard[i,1].grade == CBoard.tileOnBoard[i,2].grade ){
							if(CBoard.tileOnBoard[i,3-j]!= null && j >= 2) {
								CBoard.tileOnBoard[i,3-j].move[2] = true;
								CBoard.tileOnBoard[i,1].combine[2] = true;
							}
						}
						else{
							if(CBoard.tileOnBoard[i,0]!= null){
								if(CBoard.tileOnBoard[i,1].grade == CBoard.tileOnBoard[i,0].grade){
									if(CBoard.tileOnBoard[i,3-j] != null && j >=3){
										CBoard.tileOnBoard[i,3-j].move[2] = true;
										CBoard.tileOnBoard[i,0].combine[2] = true;
									} 
								}
							}
						}
					}
				}
				// dir - 3
				if(CBoard.tileOnBoard[0,i] == null){
					if(CBoard.tileOnBoard[j,i]!= null && j >=1 ) CBoard.tileOnBoard[j,i].move[3] = true;
				}
				else{
					if(CBoard.tileOnBoard[1,i] == null){
						if(CBoard.tileOnBoard[j,i]!= null && j >= 2) CBoard.tileOnBoard[j,i].move[3] = true;
					}
					else if( CBoard.tileOnBoard[1,i].grade == CBoard.tileOnBoard[0,i].grade ){
						if(CBoard.tileOnBoard[j,i] != null&& j >=1) {
							CBoard.tileOnBoard[j,i].move[3] = true;
							CBoard.tileOnBoard[1,i].combine[3] = true;
						}
					}
					else{
						if(CBoard.tileOnBoard[2,i] == null){
							if(CBoard.tileOnBoard[j,i] != null&& j >=3) CBoard.tileOnBoard[j,i].move[3] = true;
						}
						else if(CBoard.tileOnBoard[2,i].grade == CBoard.tileOnBoard[1,i].grade ){
							if(CBoard.tileOnBoard[j,i] != null&& j >= 2){
								CBoard.tileOnBoard[j,i].move[3] = true;
								CBoard.tileOnBoard[2,i].combine[3] = true;
							} 
						}
						else{
							if(CBoard.tileOnBoard[3,i]!= null){
								if(CBoard.tileOnBoard[3,i].grade == CBoard.tileOnBoard[2,i].grade){
									if(CBoard.tileOnBoard[j,i] != null && j >=3){
										CBoard.tileOnBoard[j,i].move[3] = true;
										CBoard.tileOnBoard[3,i].combine[3] = true;
									} 
								}
							}
						}
					}
				}
			}
		}
		SetCanMove();		
	}
	void SetCanMove(){
		board CBoard = GameObject.FindWithTag("Background").GetComponent<board>();
		for(int k = 0; k < 4 ; ++k){
			for(int l = 0 ; l < 4; ++l){
				if(CBoard.tileOnBoard[k,l] != null ){
					canMove[0] = canMove[0] || CBoard.tileOnBoard[k,l].move[0];
					canMove[1] = canMove[1] || CBoard.tileOnBoard[k,l].move[1];
					canMove[2] = canMove[2] || CBoard.tileOnBoard[k,l].move[2];
					canMove[3] = canMove[3] || CBoard.tileOnBoard[k,l].move[3];
				}
			}
		}	
	}
	void UpdateTilesOnBoard(int state){
		GameObject[] weapons;
		tile CTile;
		board CBoard = GameObject.FindWithTag("Background").GetComponent<board>();
		weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		// for 너무 많이 돌지 말자. 있는거 셋팅 10이상으로  자리하고 빼자.
		foreach (GameObject weapon in weapons) {
			int tmpGrade = 10;
			CTile = weapon.GetComponent<tile>();
			tmpGrade =  CTile.grade + tmpGrade;
			if(CTile.combined){
				CBoard.upgrade[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = true;
				Destroy(weapon);
			}
			else{
				CBoard.boardValue[ (int)CTile.tilePos.x, (int)CTile.tilePos.y] = tmpGrade;
				CBoard.tileOnBoard[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = CTile;
			}
			

		}
		for (int i = 0; i < row; ++i){
			for (int j = 0; j < col; ++j){			
				if(CBoard.boardValue[i,j] <= 10){
					CBoard.boardValue[i,j] = 0;
					CBoard.tileOnBoard[i,j] = null;
					CBoard.upgrade[i,j] = false;
				}
				else{
					CBoard.boardValue[i,j] = CBoard.boardValue[i,j] - 10;
					if(CBoard.upgrade[i,j] && state == 1){// 업그레이드
						CBoard.boardValue[i,j] = CBoard.boardValue[i,j] + 1;
						Destroy(CBoard.tileOnBoard[i,j].gameObject);
						
						int gradetmp = CBoard.boardValue[i,j]-1;
						GameObject summonTile = weaponTile[gradetmp];	
						Transform backboard = GameObject.FindWithTag("Background").transform;
						GameObject instance = Instantiate (summonTile, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
						instance.transform.SetParent (backboard);
						instance.transform.localPosition = GridToWorld(i,j);
						instance.GetComponent<tile>().tilePos.x = (float) i;
						instance.GetComponent<tile>().tilePos.y = (float) j;
						instance.GetComponent<tile>().grade = CBoard.boardValue[i,j];
						CBoard.tileOnBoard[i, j] = instance.GetComponent<tile>();

						CBoard.upgrade[i,j] = false;
					}
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
	void IsGameOver(){
		GameObject background = GameObject.FindWithTag("Background");
		board CBoard = background.GetComponent<board>();
		bool moveCheck;
        
		moveCheck = canMove[0] || canMove[1] || canMove[2]|| canMove[3];

		if(moveCheck == false){
			boardFilled = true;
		}

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
						if(CTile.combine[dir]){
							CTile.combine[dir] = false;
							CTile.combined = true;
							
						}
						break;
					case 1:
						Vtor = Vector2.right;
						if(CTile.combine[dir]){
							CTile.combine[dir] = false;
							CTile.combined = true;
						}
						break;
					case 2:
						Vtor = Vector2.up;
						if(CTile.combine[dir]){
							CTile.combine[dir] = false;
							CTile.combined = true;
						}
						break;
					case 3:
						Vtor = -Vector2.right;
						if(CTile.combine[dir]){
							CTile.combine[dir] = false;
							CTile.combined = true;
						}
						break;	
				}
				
				CTile.tilePos = CTile.tilePos + Vtor;
				weapon.transform.localPosition = GridToWorld((int)CTile.tilePos.x , (int)CTile.tilePos.y);	
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
		
		// UpdateTilesOnBoard(0);
		// UpdateTilesMoveDir();

		weaponTile = GetTileInfoOnBoard(x,y);
		if(weaponTile == null){
			return true;
		}
		
		return false;

	}
	int CanSpawnPos(){
		// 위치에 있는지 확인
		board CBoard = GameObject.FindWithTag("Background").GetComponent<board>();
		int boardpos;
		List<int> boardposlist = new List<int>();

		UpdateTilesOnBoard(0);
		UpdateTilesMoveDir();
		for (int i = 0; i < 4; ++i){
            for (int j = 0; j < 4; ++j){
				if (GetTileInfoOnBoard(i,j) == null){
					boardposlist.Add(4 * i + j);
				}   
            }
        }
		if(boardposlist.Count==0){
			Debug.Log("빈칸이 없다.");
			return 0;
		}
		else{
			boardpos = Random.Range(0 , boardposlist.Count);
			Debug.Log("Max = " + boardposlist.Count + "boardpos = " + boardpos);
			return boardposlist[boardpos];
		}
		
	}
	
	void Spawn(){
		int grade;
		int pos;
		Transform background = GameObject.FindWithTag("Background").transform;

		// 타일 랜덤하게 하나 생성 (적에 따라 다르게 생성해야 해.) todo
		grade = Random.Range(1,3); // 이렇게 하면 1,2ㄱㅏ 나오네; 짱나
		GameObject summonTile = weaponTile[grade-1];
		

		// 랜덤한 위치에 생성하기 있으면 waitingSpawn==false로 만들지 않아서 계속 되게 만든다.
		pos = CanSpawnPos ();
		int x = pos / 4;
		int y = pos % 4;

		Debug.Log(pos+ " =" + x + "," + y);
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
				UpdateTilesOnBoard(1);
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
