using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour {
	private Text Name;

	private UnityEngine.UI.Text allowList;
	private GameMgr gameMgr;

	// Use this for initialization
	void Start () {
		gameMgr = GameObject.Find ("GameManager").GetComponent<GameMgr> ();
		
	}

	// Update is called once per frame
	void Update () {

	}
	void setPlayerNum(int num){
		allowList = GameObject.Find ("AllowList").GetComponent<UnityEngine.UI.Text> ();
		allowList.text = (num + 1).ToString();
	}
}
