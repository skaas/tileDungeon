﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.Tween;

public class manager : MonoBehaviour {

	// 보드 가로,세로
	private int row = 4;
	private int col = 4;
	private float moveSenstive = 1.1f; //이정도 가면 합쳐라
	
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
	public GameObject[] monsterTile; 
	private Transform boardHolder;
	// Use this for initialization

	public bool moving; // 이동중
	public Vector2 moveCheckPos;
	public int movingDirection;

	private bool moveStart;
	private Vector2 startPosition;
	private bool inputChecker;


	void Awake () { 
		// background 로딩
		boardHolder = new GameObject("baseObject").transform;
		boardHolder.position = new Vector3 (-2.0f,-4.2f,0f);
		GameObject instance = Instantiate (board, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
        instance.transform.SetParent (boardHolder);
		instance.transform.position = new Vector3 (-2.0f,-4.2f,0f);
		
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
					MoveComplite(0);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
				
			}    	
			if(Input.GetKeyUp(KeyCode.RightArrow)){
				if(canMove[1]){
					MoveComplite(1);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
			}	
			if(Input.GetKeyUp(KeyCode.DownArrow)){
				if(canMove[2]){
					MoveComplite(2);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
			}
			if(Input.GetKeyUp(KeyCode.LeftArrow)){
				if(canMove[3]){
					MoveComplite(3);
					waitingSpawn = true;
					waitingInput = false;
				}
				else Debug.Log("못움직여");
			}
			if(Input.GetKeyDown(KeyCode.Space)){
				boardFilled = false;
				GameStart();
			}
			if(Input.GetMouseButton(0) && inputChecker){
				StartCoroutine(MouseDrag());
			}
			
			if(Input.GetMouseButtonUp(0)){
				BackToOrigin();
				inputChecker = true;
				moving = false;
				moveCheckPos = new Vector2 (0.0f, 0.0f);
				movingDirection = 100;
			}
			// else if(!inputChecker){
			// 	Debug.Log("??");
			// 	inputChecker = true;
			// }
		}			
	}
	void BackToOrigin(){
		GameObject[] weapons;
		
        weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		foreach (GameObject weapon in weapons) {
			tile CTile = weapon.GetComponent<tile>();
			CTile.BackToOrigin();
		}
	}
	void MoveNext(){
		GameObject[] weapons;
		
        weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		foreach (GameObject weapon in weapons) {
			tile CTile = weapon.GetComponent<tile>();
			CTile.TileMoveNext();
		}
	}
	void GameStart(){
		// 시작 변수
		waitingInput = true;
		waitingSpawn = false;
		
		InitValues();
		ClearBoard();
		
		
		// 생성
		Spawn();
		//
	}

	void InitValues(){

		earnedGolds = 0;
		earnedKeys = 0;
		moving = false;
		moveCheckPos = new Vector2 (0.0f,0.0f);
		//diffPos = new Vector2 (0.0f,0.0f);
		//movingDirection = 100;
		moveStart = false;
		startPosition = new Vector2 (0.0f,0.0f);
		//inputChecker = true;
		// 움직임
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
		//Debug.Log("시작 초기화");
		for (int i = 0; i < 4; ++i){
            for (int j = 0; j < 4; ++j){
				NoTileOnBoardXY(CBoard, i,j);
            }
        }
	}

	bool isCombine (tile CTile, tile DTile){
		if(CTile.hp > 0 && DTile.hp > 0) return false; //둘다 몬스터야. 합쳐지지 마
		else if(CTile.hp == 0 && DTile.hp > 0) return true; // 하나만 몬스터야 괜찮아.
		else if(CTile.hp > 0 && DTile.hp == 0) return true; //하나만 몬스터야 괜찮아.
		else if(CTile.grade == DTile.grade) return true;//둘다 몬스터 아냐 && 그래이드가 같으면
		return false;
	}
	bool IsMonster(tile CTile){
		if(CTile.hp > 0) return true;
		return false;
	}
	void NoTileOnBoardXY(board CBoard, int x, int y){
		CBoard.boardValue[x,y] = 0;   
		CBoard.tileOnBoard[x,y] = null;
		CBoard.upgradeMonsterTile[x,y] = null;
		CBoard.upgrade[x,y] = false;
		CBoard.upgradeWeaponGrade[x,y] = 0;
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

		InitValues();
		
		foreach (GameObject weapon in weapons) {
	
			CTile = weapon.GetComponent<tile>();
			TileMoveInit(CTile);
		}
		
		for (int i = 0; i < col; ++i){
			for (int j = 0; j < row; ++j){
				// dir - 0
				if(CBoard.tileOnBoard[i,0] == null){
					if(CBoard.tileOnBoard[i,j] != null && j >=1 ) CBoard.tileOnBoard[i,j].move[0] = true;
				}
				else{
					if(CBoard.tileOnBoard[i,1] == null){
						if(CBoard.tileOnBoard[i,j] != null  && j >= 2) CBoard.tileOnBoard[i,j].move[0] = true;
					}
					else if( isCombine(CBoard.tileOnBoard[i,0], CBoard.tileOnBoard[i,1])){
						if(CBoard.tileOnBoard[i,j] != null  && j >=1) {
							CBoard.tileOnBoard[i,j].move[0] = true;
							CBoard.tileOnBoard[i,1].combine[0] = true;
						}
					}
					else{
						if(CBoard.tileOnBoard[i,2] == null){
							if(CBoard.tileOnBoard[i,j] != null  && j >=3) CBoard.tileOnBoard[i,j].move[0] = true;
						}
						else if( isCombine(CBoard.tileOnBoard[i,1], CBoard.tileOnBoard[i,2])){
							if(CBoard.tileOnBoard[i,j] != null  && j >= 2){
								CBoard.tileOnBoard[i,j].move[0] = true;
								CBoard.tileOnBoard[i,2].combine[0] = true;
							} 
						}
						else{
							if(CBoard.tileOnBoard[i,3]!= null){
								if(isCombine(CBoard.tileOnBoard[i,2],CBoard.tileOnBoard[i,3])){
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
					else if( isCombine(CBoard.tileOnBoard[3,i], CBoard.tileOnBoard[2,i])){
						if(CBoard.tileOnBoard[3-j,i]!= null && j >=1) {
							CBoard.tileOnBoard[3-j,i].move[1] = true;
							CBoard.tileOnBoard[2,i].combine[1] = true;
						}
					}
					else{
						if(CBoard.tileOnBoard[1,i] == null){
							if(CBoard.tileOnBoard[3-j,i]!= null && j >=3) CBoard.tileOnBoard[3-j,i].move[1] = true;
						}
						else if(isCombine(CBoard.tileOnBoard[2,i], CBoard.tileOnBoard[1,i])){
							if(CBoard.tileOnBoard[3-j,i]!= null && j >= 2){
								CBoard.tileOnBoard[3-j,i].move[1] = true;
								CBoard.tileOnBoard[1,i].combine[1] = true;
							} 
						}
						else{
							if(CBoard.tileOnBoard[0,i]!= null){
								if(isCombine( CBoard.tileOnBoard[0,i], CBoard.tileOnBoard[1,i])){
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
					else if( isCombine(CBoard.tileOnBoard[i,2], CBoard.tileOnBoard[i,3])){
						if(CBoard.tileOnBoard[i,3-j]!= null && j >=1){
							CBoard.tileOnBoard[i,3-j].move[2] = true;
							CBoard.tileOnBoard[i,2].combine[2] = true;
						} 
					}
					else{
						if(CBoard.tileOnBoard[i,1] == null){
							if(CBoard.tileOnBoard[i,3-j]!= null && j >=3) CBoard.tileOnBoard[i,3-j].move[2] = true;
						}
						else if(isCombine(CBoard.tileOnBoard[i,1], CBoard.tileOnBoard[i,2]) ){
							if(CBoard.tileOnBoard[i,3-j]!= null && j >= 2) {
								CBoard.tileOnBoard[i,3-j].move[2] = true;
								CBoard.tileOnBoard[i,1].combine[2] = true;
							}
						}
						else{
							if(CBoard.tileOnBoard[i,0]!= null){
								if(isCombine(CBoard.tileOnBoard[i,1], CBoard.tileOnBoard[i,0]) ){
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
					else if( isCombine(CBoard.tileOnBoard[1,i], CBoard.tileOnBoard[0,i]) ){
						if(CBoard.tileOnBoard[j,i] != null&& j >=1) {
							CBoard.tileOnBoard[j,i].move[3] = true;
							CBoard.tileOnBoard[1,i].combine[3] = true;
						}
					}
					else{
						if(CBoard.tileOnBoard[2,i] == null){
							if(CBoard.tileOnBoard[j,i] != null&& j >=3) CBoard.tileOnBoard[j,i].move[3] = true;
						}
						else if(isCombine(CBoard.tileOnBoard[2,i], CBoard.tileOnBoard[1,i]) ){
							if(CBoard.tileOnBoard[j,i] != null&& j >= 2){
								CBoard.tileOnBoard[j,i].move[3] = true;
								CBoard.tileOnBoard[2,i].combine[3] = true;
							} 
						}
						else{
							if(CBoard.tileOnBoard[3,i]!= null){
								if(isCombine(CBoard.tileOnBoard[3,i], CBoard.tileOnBoard[2,i]) ){
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
		CheckPlayerCanMove();		
	}

	void CheckPlayerCanMove(){
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

	bool CheckTileUpgrade(tile CTile){
		if(CTile.combined){
			return true;
		}
		return false;
		
	}
	void SetUpgradeBoard(board CBoard, tile CTile){
		CBoard.upgrade[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = true;
		CBoard.upgradeWeaponGrade[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = CTile.grade;
		if(CTile.hp > 0) {
			
			CBoard.upgradeMonsterTile[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = CTile;
		}
	}

	void SetThisTileInBoardValue(board CBoard, tile CTile, int val){
		CBoard.boardValue[ (int)CTile.tilePos.x, (int)CTile.tilePos.y] = val;
	}
	void SetInBoardValue(board CBoard, int x, int y, int val){
		CBoard.boardValue[x, y] = val;
	}
	void SetThisTileInTileOnBoard(board CBoard, tile CTile){
		CBoard.tileOnBoard[(int)CTile.tilePos.x, (int)CTile.tilePos.y] = CTile;
	}
	void UpdateTilesOnBoard(int state){
		GameObject[] weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		GameObject boardObject = GameObject.FindWithTag("Background");
		board CBoard = boardObject.GetComponent<board>();
		tile CTile;
		
		// for 너무 많이 돌지 말자. 있는거 셋팅 10이상으로  자리하고 빼자.
		foreach (GameObject weapon in weapons) {
			int tmpVal = 10;
			CTile = weapon.GetComponent<tile>();	
			
			if( CheckTileUpgrade(CTile) ){
				SetUpgradeBoard(CBoard, CTile);
				Destroy(CTile.gameObject);
			}
			else{
				tmpVal =  1 + tmpVal;
				SetThisTileInBoardValue(CBoard,CTile,tmpVal);
				SetThisTileInTileOnBoard(CBoard,CTile);
			}
		}
		for (int i = 0; i < row; ++i){
			for (int j = 0; j < col; ++j){
				if(CBoard.boardValue[i,j] <= 10){
					NoTileOnBoardXY(CBoard,i,j);
				}
				else{
					SetInBoardValue(CBoard,i,j, CBoard.boardValue[i,j] - 10);
					if(CBoard.upgrade[i,j] && state == 1){// 업그레이드
						
						if(IsMonster(CBoard.tileOnBoard[i,j])){
							int nowHP = CBoard.tileOnBoard[i,j].hp - CBoard.upgradeWeaponGrade[i,j];
							if ( nowHP <= 0){
								Destroy(CBoard.tileOnBoard[i,j].gameObject); 
								NoTileOnBoardXY(CBoard,i,j);
								//드롭 아이템
							}
							else{
								CBoard.tileOnBoard[i,j].hp = nowHP;
								Debug.Log("맞고 죽지 않음 HP = " + nowHP);
							}
							CBoard.upgradeWeaponGrade[i,j] = 0;
						}
						else{
							Debug.Log("(" + i+"," + j+ ")");

							if(CBoard.upgradeMonsterTile[i,j] != null){ //없어진 타일이 몬스터다
								int nowHP;
								nowHP = CBoard.upgradeMonsterTile[i,j].hp - CBoard.tileOnBoard[i,j].grade;
								if ( nowHP <= 0){
									Destroy(CBoard.tileOnBoard[i,j].gameObject); 
									NoTileOnBoardXY(CBoard,i,j);
									Debug.Log("몬스터 죽음");
									//드롭 아이템
								}
								else{
									Destroy(CBoard.tileOnBoard[i,j].gameObject);
									SummonMosterTile(boardObject,CBoard.upgradeMonsterTile[i,j],i,j);
									Debug.Log("맞고 죽지 않음 HP = " + nowHP);
								}
								CBoard.upgradeMonsterTile[i,j] = null;
							}
							else{
								int nowGrade = CBoard.tileOnBoard[i,j].grade;
								Debug.Log("업그레이드: nowGrade = " + nowGrade);
								Destroy(CBoard.tileOnBoard[i,j].gameObject);
								SummonWeaponTile(boardObject,nowGrade ,i,j);
							}
							
						}
						
						CBoard.upgrade[i,j] = false;
					}
				}
			}
		}
	}
	
	void SummonTile(GameObject boardObject , int tileArry, int x, int y){
		int isMonster;
		isMonster = (int) Random.Range(0,2);
		GameObject summonTile;
		GameObject instance;
		int hp = 0; //몬스터일때만
		int attackValue = 0; //몬스터 일때만 
		int grade = tileArry + 1;
		if(isMonster == 0){
			summonTile = weaponTile[tileArry];
		}
		else{
			summonTile = monsterTile[0];
			hp = 1;
			attackValue = 1;
			grade = 100;
		}
		
		instance = Instantiate (summonTile, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
		tile CTile = instance.GetComponent<tile>();
		board CBoard = boardObject.GetComponent<board>();
		
		instance.transform.SetParent (boardObject.transform);
		Vector3 _tmpPos = Vector3.zero;
		Debug.Log("movingDirection = " + movingDirection);
		//y= 7 -3 , x = -1 .4
		if(movingDirection == 0){
			_tmpPos = GridToWorld(x,4);
		}
		else if (movingDirection == 1){
			_tmpPos = GridToWorld(-1,y);
		}
		else if (movingDirection == 2){
			_tmpPos = GridToWorld(x,-1);
		}
		else if (movingDirection == 3){
			_tmpPos = GridToWorld(4,y);
		}
		else{
			_tmpPos = GridToWorld(x,y);
		}
		movingDirection = 100;
		instance.gameObject.Tween("MoveCircle", _tmpPos, GridToWorld(x,y), 0.5f, TweenScaleFunctions.CubicEaseIn, (t) =>
            {
                // progress
                instance.gameObject.transform.localPosition = t.CurrentValue;
            }, (t) =>
            {
            });

		//instance.transform.localPosition = GridToWorld(x,y);
		CTile.tilePos.x = (float) x;
		CTile.tilePos.y = (float) y;
		CTile.grade = grade;
		CTile.hp = hp;
		CTile.attackValue = attackValue;

		SetThisTileInBoardValue(CBoard,CTile,1);
		SetThisTileInTileOnBoard(CBoard,CTile);

	}
	void SummonWeaponTile(GameObject boardObject , int tileArry, int x, int y){
		
		GameObject summonTile;
		GameObject instance;
		int grade = tileArry + 1;
		
		summonTile = weaponTile[tileArry];
		
		instance = Instantiate (summonTile, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
		tile CTile = instance.GetComponent<tile>();
		board CBoard = boardObject.GetComponent<board>();
		
		instance.transform.SetParent (boardObject.transform);
		instance.transform.localPosition = GridToWorld(x,y);
		CTile.tilePos.x = (float) x;
		CTile.tilePos.y = (float) y;
		CTile.grade = grade;

		SetThisTileInBoardValue(CBoard,CTile,1);
		SetThisTileInTileOnBoard(CBoard,CTile);

	}
	void SummonMosterTile(GameObject boardObject , tile Ctile, int x, int y){
		GameObject summonTile;
		GameObject instance;

		int hp = Ctile.hp; //몬스터일때만
		int attackValue = Ctile.attackValue; //몬스터 일때만 
		int grade = Ctile.grade;
		
		summonTile = monsterTile[grade-100];
		
		instance = Instantiate (summonTile, new Vector3 (0f,0f,0f), Quaternion.identity) as GameObject;
		tile CTile = instance.GetComponent<tile>();
		board CBoard = boardObject.GetComponent<board>();
		
		instance.transform.SetParent (boardObject.transform);
		instance.transform.localPosition = GridToWorld(x,y);
		CTile.tilePos.x = (float) x;
		CTile.tilePos.y = (float) y;
		CTile.grade = grade;
		CTile.hp = hp;
		CTile.attackValue = attackValue;

		SetThisTileInBoardValue(CBoard,CTile,1);
		SetThisTileInTileOnBoard(CBoard,CTile);

	}
	// 타일 움직이기xw
	void IsGameOver(){
		GameObject background = GameObject.FindWithTag("Background");
		board CBoard = background.GetComponent<board>();
		bool moveCheck;
        
		moveCheck = canMove[0] || canMove[1] || canMove[2]|| canMove[3];

		if(moveCheck == false){
			boardFilled = true;
		}

	}
	void MoveingTile(Vector2 diffpos, int dir){
		GameObject[] weapons;
		
        weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		foreach (GameObject weapon in weapons) {
			tile CTile = weapon.GetComponent<tile>();
			if (CTile.move[dir]){	
				CTile.TileMove(diffpos,dir);
			}
		}
	}
	public void MoveComplite(int dir) {
		//0:up, 1:Right, 2:down, 3:Left
		
		GameObject[] weapons;
        weapons = GameObject.FindGameObjectsWithTag("WeaponTile");
		inputChecker = false;
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
		// 움직임이 종료 되었음. 여기 좀 오류가 나네;;
		//UpdateTilesOnBoard(0);
		//UpdateTilesMoveDir();
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
		if(GetTileInfoOnBoard(x,y) == null) return true;
		return false;
	}
	int CanSpawnPos(){
		// 위치에 있는지 확인
		board CBoard = GameObject.FindWithTag("Background").GetComponent<board>();
		int boardpos;
		List<int> boardposlist = new List<int>();

		//움직임이 끝난 타이밍을 잡고 싶은데 애매하다. 그래서 생성하기 전에 업데이트 움직임 --> 생성
		// 찾
		UpdateTilesOnBoard(0);
		UpdateTilesMoveDir();
		//----------------------------------------------------------------------
		for (int i = 0; i < 4; ++i){
            for (int j = 0; j < 4; ++j){
				if (GetTileInfoOnBoard(i,j) == null){
					boardposlist.Add(4 * i + j);
				}   
            }
        }
		if(boardposlist.Count==0){
			Debug.Log("빈칸이 없다.");
			// 가끔씩 여기 걸리는데 찾기 귀찮아서 예외처리.
			return 0;
		}
		else{
			//빈칸중에 랜덤하게 리턴
			boardpos = Random.Range(0 , boardposlist.Count);
			return boardposlist[boardpos];
		}
		
	}
	
	void Spawn(){
		int grade;
		int pos;
		GameObject boardObject = GameObject.FindWithTag("Background");
		Transform background = boardObject.transform;

		// 타일 랜덤하게 하나 생성 (적에 따라 다르게 생성해야 해.) todo
		grade = Random.Range(1,3); // 이렇게 하면 1,2ㄱㅏ 나오네; 짱나
		

		// 랜덤한 위치에 생성하기 있으면 waitingSpawn==false로 만들지 않아서 계속 되게 만든다.
		pos = CanSpawnPos ();
		int x = pos / 4;
		int y = pos % 4;

		if(CanSpawn(x,y)){
			//타일을 뿌려줌
			SummonTile(boardObject, grade-1,x,y);

			//혹시 새로운게 두 개 이상일 경우도 있으니. 테스트도 포함해서.
			newTileCount++;
			if(newTileMaxCount <= newTileCount){
				// 생성이 종료 되었음	
				newTileCount = 0;
				waitingSpawn = false;
				waitingInput = true;
				UpdateTilesOnBoard(1);
				UpdateTilesMoveDir();
			}
		}
	}
	
	public static Vector3 GridToWorld(int x, int y) {
		// 0,0 = 0.3 , 6.6 
		return new Vector3(1.2f * x  + 0.3f, 6.6f - (1.2f *y) , 0f);
	}
	
	public static Vector2 WorldToGrid(float x, float y) {
		return new Vector2((x - 0.3f)/1.2f, (6.6f - y)/1.2f);
	}
	IEnumerator MouseDrag(){ 
		Vector2 mouseDragPosition = new Vector2(Input.mousePosition.x , Input.mousePosition.y);
		Vector2 worldObjectPosition = Camera.main.ScreenToWorldPoint(mouseDragPosition);
		if(moving){
			if( moveCheckPos != worldObjectPosition){
				Vector2 diffPos;				
				diffPos = worldObjectPosition - moveCheckPos;
				//Debug.Log(worldObjectPosition + " - " + moveCheckPos +";;update diffPos= "+ diffPos );
				if(diffPos.x != 0.0f || diffPos.y != 0.0f){
					if(movingDirection == 100){
						startPosition = worldObjectPosition;
						if(diffPos.y > 0.0f && diffPos.x > -0.5f && diffPos.x < 0.5f)
						{
							movingDirection = 0;
						}
						else if(diffPos.y < 0.0f   && diffPos.x > -0.5f && diffPos.x < 0.5f)
						{
							movingDirection = 2;
						}
						//swipe left
						else if(diffPos.x < 0.0f  && diffPos.y > -0.5f && diffPos.y < 0.5f)
						{
							movingDirection = 3;
						}
						//swipe right
						else if(diffPos.x > 0.0f && diffPos.y > -0.5f && diffPos.y < 0.5f)
						{
							movingDirection = 1;
						}
					}
					else{
						Vector2 checkDistance = moveCheckPos - startPosition;
						//Debug.Log("얼마나 움직였나?" + checkDistance );
						if(movingDirection == 0 && checkDistance.y > moveSenstive){
							moving = false;
							waitingSpawn = true;
							waitingInput = false;
							MoveComplite(movingDirection);
						}
						else if(movingDirection == 2 && checkDistance.y < -moveSenstive){
							moving = false;
							waitingSpawn = true;
							waitingInput = false;
							MoveComplite(movingDirection);
						}
						else if(movingDirection == 1 && checkDistance.x > moveSenstive){
							moving = false;
							waitingSpawn = true;
							waitingInput = false;
							MoveComplite(movingDirection);
						}
						else if(movingDirection == 3 && checkDistance.x < -moveSenstive){
							moving = false;
							waitingSpawn = true;
							waitingInput = false;
							MoveComplite(movingDirection);
						}
						else{
							MoveingTile(diffPos, movingDirection);
						}
					}						
					moveCheckPos = worldObjectPosition;
				}
				
			}
			else{
				moveCheckPos = worldObjectPosition;
			}
			
		}
		else{
			moving = true;
			moveCheckPos = worldObjectPosition;
		}
				
			
		yield return null;
	}
		
}

