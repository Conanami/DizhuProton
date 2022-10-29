using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public enum GameProgress
{
    WaitForPlayer,
    FullPlayer,
    SeatAssigned,
    WaitForCard,
    CardDealed,
    CallLord,
    PlayInProgress,
    EndGame,
}
public class NetworkLoader : MonoBehaviourPunCallbacks
{
    private float lastRefreshTime;
    public float refreshInterval;

    public GameObject nameUI;
    public GameObject userList;
    // Start is called before the first frame update

    public static NetworkLoader Instance;    //单例模式

    private GameProgress currentStatus;
    public GameProgress CurrentStatus
    {
        get
        {
            return currentStatus;
        }
        set
        {
            currentStatus = value;
            localStatus = value;
            //UpdateProperties("gameProgress", value.ToString());
        }
    }

    public GameProgress localStatus;        //记录一下本地的状态

    private int turnPlayer;
    public int TurnPlayer
    {
        get
        {
            return turnPlayer;
        }
        set
        {
            turnPlayer = value;
            UpdateProperties("turnPlayer", value.ToString());
        }
    }

    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        PhotonNetwork.ConnectUsingSettings();
        
    }

    // Update is called once per frame
    
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to master");
        //PhotonNetwork.JoinLobby();


        StartCoroutine(JoinLobby());
        //PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions() { MaxPlayers = 4 }, default);
        
    }

    IEnumerator JoinLobby()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Try to Join Lobby");
        PhotonNetwork.JoinLobby();
        nameUI.SetActive(true);
        Application.runInBackground = true;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        userList.SetActive(true);
        userList.GetComponentInChildren<UserList>().UpdateList();
    }

    public void UpdateProperties(string key, string value)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(key, value);                          
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        Debug.Log(key+":"+value);
    }

    public string GetProperties(string key)
    {
        if (PhotonNetwork.CurrentRoom == null) return "";
        return (string)PhotonNetwork.CurrentRoom.CustomProperties[key];
    }

    public int GetMyPlayerKey()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public List<int> Str2IntList(string str)
    {
        List<int> rtlist = new List<int>();
        string[] tmplist = str.Split(',');
        foreach (string s in tmplist)
        {
            rtlist.Add(int.Parse(s));
        }
        return rtlist;
    }

    public Dictionary<int,Player> GetPlayerList()
    {
        if(PhotonNetwork.CurrentRoom!=null)
            return PhotonNetwork.CurrentRoom.Players;
        return null;
    }

    public int NextPlayer(int currentTurn)   //得到下一个行动的玩家
    {
        if (currentTurn < 3)
            return currentTurn + 1;
        else
            return 0;
    }

    public bool myTurn() //如果是true，就说明轮到我了
    {
        string strSeatlist = GetProperties("seatList");
        List<int> seatList = Str2IntList(strSeatlist);
        int turn = int.Parse(GetProperties("turnPlayer"));
        int myKey = GetMyPlayerKey();
        return myKey == seatList[turn];
    }
}
