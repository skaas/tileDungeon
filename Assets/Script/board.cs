using UnityEngine;
using System.Collections;

public class board : MonoBehaviour {

	public int[,] boardValue = new int [4,4]; // 위에 있냐없냐?
	

	public tile[,] tileOnBoard = new tile[4,4];
	public bool[,] upgrade = new bool[4,4];
	public int[,] upgradeWeaponGrade = new int [4,4]; // 업그레이드 할때 Detroy타일의 공격력
	public tile[,] upgradeMonsterTile =  new tile[4,4];  // 업그레이드 할때 Detroy타일의 HP
	// Use this for initialization
	void Awake () {
		for (int i = 0; i < 4; ++i){
            for (int j = 0; j < 4; ++j){
				Debug.Log("여기 초기화는 문제 없고?");
				upgradeWeaponGrade[i,j] = 0;
				upgradeMonsterTile[i,j] = null;
                boardValue[i,j] = 0;   
				tileOnBoard[i,j] = null;
				upgrade[i,j] = false;
            }
        }
	}
}
