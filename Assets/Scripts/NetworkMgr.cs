using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkMgr : MonoBehaviour
{
    GameObject GameCanvas;
    GameObject MainPanel;
    GameObject RoomPanel;
    GameObject ReadyBtn;
    GameObject StartBtn;

    public Transform[] Points;
    public Transform[] VaccinePoints;
    public int point;
    public string ip;
    public string nick;
    public string defaultNick = "Player";
    private const int port = 30000;
    private bool _useNat = false;
    public bool isGameStart = false;
    public InputField nickField;
    public InputField ipField;
    public GameObject player;
    private NetworkView _netWorkView;
    public bool isStartBtnClick;

    /// <summary>
    /// 감염된사람을 저장하기위한 infectedman, 감염자 출현 isinfetedmango
    /// 플레이어의 리스트 Lplayer->캐릭터오브젝트가 있는 스크립트에서 리스트에 추가해주어야한다 넣어준다
    /// 감염자의 위치를 확인하는 InfetedVetor
    /// </summary>
    public int Infectedman;
    public List<GameObject> Lplayer;
    private bool IsInfectedManGo = false;
    private Vector3 InfectedVetor;

    /// ///////////////////

    public int clicount=0;
    public static int allPlayer;

    ///방장이 게임스타트 버튼 클릭시 직업 랜덤하게 주어줄 함수
    ///서버인지 확인후 플레이어 카운트트만큼의 범위로 랜덤값줌
    ///그후 자신의 위치를 주어 serialuze로Infectedman,infectedvetor,IsInfectedManGo공유
    /// 

    public void OnClickStartBtn()
    {
        Network.InitializeServer(20, port, _useNat);

   
    }

    public void OnClickConnectBtn()
    {
        if (ipField.text == "")
            Network.Connect("192.168.115.48", port);

        else
            Network.Connect(ip, port);
    }

    public void OnclickGameStartBtn()
    {
        _netWorkView.RPC("BroadcastStart", RPCMode.All);
        clicount = (Network.connections.Length)+1;

        allPlayer = clicount;

        StartCoroutine(this.RanInfect());


 
    }
    IEnumerator RanInfect()
    {
        if (Network.isServer)//서버인지 확인후 랜덤값부분
        {

            while (Lplayer.Count != clicount)
            {
                yield return new WaitForSeconds(1f);
            }
            for (int i = 0; i < clicount; i++)
            {
                Lplayer[i].GetComponent<PlayerCtrl>().callmynumset(i) ;
            }

            int ran = Random.Range(0, Lplayer.Count - 1);
            Infectedman = ran;

            InfectedVetor = Lplayer[Infectedman].transform.localPosition;

            IsInfectedManGo = true;

        }
        yield return null;
    }
    public void IpChange(string ip)
    {
        this.ip = ip;
    }

    public void NickChange()
    {
        if (nickField.text == "")
        {
            nick = defaultNick;
           
        }
        else
        {
            nick = nickField.text;
     
        }

        Debug.Log("닉네임이 " + nick + " 로 변경되었습니다.");
        BroadcastNickName();
    }

    private void OnGUI()
    {

        if (Network.peerType == NetworkPeerType.Server)
        {
            GUI.Label(new Rect(20, 90, 200, 25), "Initialization Server...");
            GUI.Label(new Rect(20, 120, 200, 25), "Client Count = " + Network.connections.Length.ToString());
        }
        if (Network.peerType == NetworkPeerType.Client)
        {
            GUI.Label(new Rect(20, 90, 200, 25), "Connected to Server");
        }
    }

    void Awake()
    {
        _netWorkView = GetComponent<NetworkView>();
        Points = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        VaccinePoints = GameObject.Find("ItemSpawnPoint").GetComponentsInChildren<Transform>();
    }

    // Use this for initialization
    void Start()
    {
        GameCanvas = GameObject.Find("GameCanvas");
        MainPanel = GameObject.Find("MainPanel");
        RoomPanel = GameObject.Find("RoomPanel");
        StartBtn = GameObject.Find("StartBtn");
        ReadyBtn = GameObject.Find("ReadyBtn");


        GameCanvas.SetActive(false);
        RoomPanel.SetActive(false);
        StartBtn.SetActive(false);

        nick = defaultNick;

        isStartBtnClick = false;

        //RoleSet(Network.connections.Length);
        //  _netWorkView.RPC("RoleSet", RPCMode.Others, Network.connections.Length);
    }


    // Update is called once per frame
    void Update()
    {
        
        point = Random.Range(1, VaccinePoints.Length);

        InfectedManstart();//////////////////메롱
 

        if (isStartBtnClick)
        {
           
 
            CreatePlayer();
            
            isStartBtnClick = false;
             
        }
    }

public void BroadcastNickName()
    {

     //   _netWorkView.RPC("UpdateNickName", RPCMode.All, nick);
    }

    public void SendNickNameTo(NetworkPlayer aPlayer)
    {
     //   _netWorkView.RPC("UpdateNickName", aPlayer, nick);
    }

    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("방장 도망감 님들 나가셈");
    }

    void OnServerInitialized()
    {
        Debug.Log("OnServerInitialized()");

        StartBtn.SetActive(true);
        MainPanel.SetActive(false);
        RoomPanel.SetActive(true);
        NickChange();
        _netWorkView.RPC("Roomin", RPCMode.All, nick);

        Debug.Log("게임 대기중! 당신의 닉네임은 " + nick + " 입니다");
        Debug.Log("방장이 게임을 시작해야되요 어서 시작하죠.");
    }

    void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer()");

        MainPanel.SetActive(false);
        RoomPanel.SetActive(true);
        NickChange();
        _netWorkView.RPC("Roomin", RPCMode.All, nick);

        Debug.Log("게임 대기중! 당신의 닉네임은 " + nick + " 입니다");
        Debug.Log("방장이 게임을 시작해야되요 어서 시작하시라 그래요.");
    }

    [RPC]
    void Roomin(string nick)
    {
        Debug.Log(nick + "님이 입장하셨습니다.");
    }

    [RPC]
    void BroadcastStart()
    {
        RoomPanel.SetActive(false);
        GameCanvas.SetActive(true);
        isGameStart = true;
        isStartBtnClick = true;
    }

    void CreatePlayer()
    {
            int idx = Random.Range(1, Points.Length);
            Vector3 pos = new Vector3(Points[idx].position.x, Points[idx].position.y, Points[idx].position.z);
            GameObject p = (GameObject)Network.Instantiate(player, pos, Quaternion.identity, 0);
            Debug.Log("CreatePlayer 호출");
            
        p.GetComponent<PlayerCtrl>().callcnikchage(nick);

    }

    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.Serialize(ref Infectedman);
            stream.Serialize(ref InfectedVetor);
            stream.Serialize(ref IsInfectedManGo);
        }
        else
        {
            stream.Serialize(ref Infectedman);
            stream.Serialize(ref InfectedVetor);
            stream.Serialize(ref IsInfectedManGo);
        }
    }



    //////////
    /// <summary>
    /// isinfectedmango를 참일때 벡터값을 통해 감염자 게임오브젝트 구별부분
    /// update부분에 구현되야함
    /// </summary>
    void InfectedManstart()
    {
 

        if (IsInfectedManGo)
        {

            for (int i = 0; i < Lplayer.Count; i++)
            {

                int imsi = Lplayer[i].GetComponent<PlayerCtrl>().mynum;
                if (imsi == Infectedman)
                {

                    Lplayer[i].GetComponent<PlayerCtrl>().imInfect = true;          //player의 스크립트에 변수있어야한다.         
                    StartCoroutine(Infectedfalse());

                    break;
                }
            }
        }
    }
    IEnumerator Infectedfalse()
    {
        yield return new WaitForSeconds(0.2f);
        IsInfectedManGo = false;

    }
}
