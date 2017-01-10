﻿using UnityEngine;
using System.Collections;

public class board : MonoBehaviour {

	public int[,] boardValue = new int [4,4];
	public tile[,] tileOnBoard = new tile[4,4];
	// Use this for initialization
	void Awake () {
		for (int i = 0; i < 4; ++i){
            for (int j = 0; j < 4; ++j){
                boardValue[i,j] = 0;   
				tileOnBoard[i,j] = null;
            }
        }
	}
}