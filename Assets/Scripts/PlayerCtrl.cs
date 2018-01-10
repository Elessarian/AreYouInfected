using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private Transform playerTr;
    public GameObject Vaccine;

    private GameMgr gameMgr;
    private UnityEngine.UI.Text vaccinePanel;
    private UnityEngine.UI.Text statePanel;
    private NetworkMgr networkMgr;
    private UIMgr uiMgr;
    private NetworkView networkView;
    public float getDist = 2.0f;
    private float dist;
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;
    public bool imInfect = false;//감염되었는지 확인하는변수 networkMgr에서 값이 변화됨
     public  string nick;


    public int mynum = -1;
    public bool isDist = false;
    public bool isGetVaccine = false;
    private void Awake()
    {
        networkView = GetComponent<NetworkView>();
    }
    void Start()
    {
        playerTr = GetComponent<Transform>();
        networkMgr = GameObject.Find("NetworkManager").GetComponent<NetworkMgr>();
        
        networkMgr.Lplayer.Add(this.gameObject);//NetworkMgr에 Lplayer리스트에 자기자신 추가
        if (!networkView.isMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
        }
        else
        {
            Renderer[] mych = GetComponentsInChildren<Renderer>();
            foreach (Renderer ga in mych)
            {

                ga.enabled = false;

            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (networkView.isMine)
        {
            if (GameMgr.vaccineExist)
            {
                Vaccine = GameObject.FindGameObjectWithTag("Vaccine");
                dist = Vector3.Distance(playerTr.position, Vaccine.transform.position);


                if (dist <= getDist)
                {

                    if (Input.GetKeyDown(KeyCode.E) && !isGetVaccine)
                    {
                        Debug.Log("E클릭");
                        networkView.RPC("PushEKey", RPCMode.All);
                        GameMgr.callVacinfalse(false);
                    }
                }
            }
            if (networkMgr.isGameStart)
            {
                statePanel = GameObject.Find("State").GetComponent<UnityEngine.UI.Text>();
                if (GameMgr.vaccineExist)
                {
                    vaccinePanel = GameObject.Find("Vaccine").GetComponent<UnityEngine.UI.Text>();
                }
                if (networkMgr.isGameStart)
                {
                    if (isGetVaccine)
                    {
                        vaccinePanel.text = "<color=#0000ff>Have</color>";
                    }
                    if (vaccinePanel)
                    {
                        if (!isGetVaccine)
                        {
                            vaccinePanel.text = "<color=#ff0000>None</color>";
                        }
                    }
                }
                if (networkMgr.isGameStart && imInfect)
                {
                    statePanel.text = "<color=#ff00ff>infecter</color>";
                }
                else
                {
                    statePanel.text = "<color=#00ff00>Citizen</color>";
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, currPos) >= 2.0f)
            {
                transform.position = currPos;
                transform.rotation = currRot;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f);
            }
        }
    }

    [RPC]
    void PushEKey()
    {
        isGetVaccine = true;
    }


    public void callcnikchage(string newnick)
    {
        Debug.Log(newnick);
      
       networkView.RPC("nickChange", RPCMode.All, newnick);
       
    }



    [RPC]
    public void nickChange(string newnick)
    {
        nick = newnick;
        Debug.Log("왔다");
    }

    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            


        }
        else
        {
            Vector3 revPos = Vector3.zero;
            Quaternion revRot = Quaternion.identity;
            int _animState = 0;

            stream.Serialize(ref revPos);
            stream.Serialize(ref revRot);
            

            currPos = revPos;
            currRot = revRot;

        }
    }

    public void callmynumset(int a)
    {
        this.networkView.RPC("mynumset", RPCMode.All, a);
    }
    [RPC]
    void mynumset(int a)
    {
        mynum = a;
    }

    void OnDamage()
    {
        networkView.RPC("YouDied", RPCMode.All);
    }

    [RPC]
    public void YouDied()
    {
        Debug.Log(mynum.ToString() + "Die");
        Destroy(this.gameObject);
        NetworkMgr.allPlayer--;
    }

}
