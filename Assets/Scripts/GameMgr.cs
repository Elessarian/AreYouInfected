using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public static GameObject[] points;
    GameObject EndingPanel;

    static private NetworkView _netWorkView;

    public float spawnTime = 60.0f;
    public static bool vaccineExist = false;
    public bool isGameOver = false;
    public static int vaccinePos;
    public Text winner;


    // 아래는 플레이어 리스트 함수에 관한 변수 입니다.
    private List<NetworkMgr> m_Players = new List<NetworkMgr>();


    // Use this for initialization
    void Start()
    {
        EndingPanel = GameObject.Find("EndingPanel");
        EndingPanel.SetActive(false);


        points = GameObject.FindGameObjectsWithTag("Vaccine");
        _netWorkView = GetComponent<NetworkView>();

        for (int i = 0; i < points.Length; i++)
            points[i].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(NetworkMgr.allPlayer == 1)
        {
            winner.text = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>().nick;
            EndingPanel.SetActive(true);

        }

    }



    //플레이어 리스트생성함수입니다.

    private void OnPlayerConnected(NetworkPlayer player)
    {
        foreach (NetworkMgr N in m_Players)
            N.SendNickNameTo(player);
    }


    public void OnClickStartGameBtn()
    {
        if (Network.isServer)
        {
            StartCoroutine(this.SpawnVaccine());
        }
    }



    // 백신 생성
    [RPC]
    public void CreateItem(int vc, bool ve)
    {
        vaccinePos = vc;
        points[vc].SetActive(true);
        vaccineExist = ve;
        Debug.Log("백신포지션 : " + vc);
        Debug.Log("백신 생성 : " + vaccineExist);
    }

    // 백신 초기화
    void InitItem()
    {

    }

    IEnumerator SpawnVaccine()
    {
        Debug.Log("코돔");
        while (!isGameOver)
        {
            Debug.Log("와돔");
            if (!vaccineExist)
            {
                Debug.Log("이돔");
                yield return new WaitForSeconds(spawnTime);
                vaccinePos = Random.Range(0, 2);
                vaccineExist = true;
                Debug.Log("현재 백신존재값 : " + vaccineExist);
                _netWorkView.RPC("CreateItem", RPCMode.All, vaccinePos, vaccineExist);

            }
            else
            {
                yield return null;
            }
        }
    }

    public static void callVacinfalse(bool bl)
    {
       // vaccineExist=bl;
        _netWorkView.RPC("Vaccinefalse", RPCMode.All,vaccinePos, bl);
        Debug.Log("콜벡신");
        Debug.Log("현재 백신존재값 : "+vaccineExist);
    }


    [RPC]
    public void Vaccinefalse(int vp, bool ve)
    {
        vaccineExist = ve;
        points[vp].SetActive(false);
        Debug.Log("백신 비활성화");

    }




}
