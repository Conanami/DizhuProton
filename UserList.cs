using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class UserList : MonoBehaviourPunCallbacks
{
    public GameObject playerNamePrefab;
    public Transform nameList;
    public GameObject handCard;
    public float cdtime;
    private float lastRefresh;
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("UserList call on player entered Room");
        UpdateList();
        if(PhotonNetwork.CurrentRoom.Players.Count==4)
        {
            Debug.Log("人已到齐，可以开始发牌。");
            NetworkLoader.Instance.CurrentStatus = GameProgress.FullPlayer;
        }
        
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdateList();
        if (PhotonNetwork.CurrentRoom.Players.Count < 3)
        {
            Debug.Log("人不齐，等人来重新摸位子");
            ResetAll();
        }
        
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        //重试加入房间
        StartCoroutine(ReJoinRoom());
    }

    IEnumerator ReJoinRoom()
    {
        int cnt = 0;
        while(cnt<15 && !PhotonNetwork.InRoom)
        {
            Debug.Log("Try to reconnect");
            yield return Reconnect();
            cnt++;
        }
        
    }
    IEnumerator Reconnect()
    {
        PhotonNetwork.ConnectUsingSettings();
        yield return new WaitForSeconds(5f);

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };

        string[] keys = { "users", "conditions" };

        options.CustomRoomPropertiesForLobby = keys;

        PhotonNetwork.JoinOrCreateRoom("Room", options, default);
        //PhotonNetwork.NickName = playerName.text;
        if (PhotonNetwork.InRoom)
        {
            Debug.Log(PhotonNetwork.NickName + " reJoin room");
        }
        //this.gameObject.SetActive(false);
    }
    void ResetAll()    //重置所有变量
    {
        //Time.timeScale = 1;
        NetworkLoader.Instance.CurrentStatus = GameProgress.WaitForPlayer;
        NetworkLoader.Instance.UpdateProperties("gameProgress", GameProgress.WaitForPlayer.ToString());
        NetworkLoader.Instance.UpdateProperties("seatList", null);
        handCard.GetComponent<HandCard>().ClearCards();
        NetworkLoader.Instance.UpdateProperties("card", null);
        NetworkLoader.Instance.UpdateProperties("lord", null);
        NetworkLoader.Instance.UpdateProperties("betsize", null);
        NetworkLoader.Instance.UpdateProperties("candidate", null);
        NetworkLoader.Instance.UpdateProperties("calledCount", null);
        PlayManager.Instance.IsSeated = false;
        PlayManager.Instance.GotHandCard = false;
        PlayManager.Instance.GotDipai = false;
        PlayManager.Instance.callLord.SetActive(false);
        PlayManager.Instance.callLord.GetComponent<CallLord>().called = false;
        PlayManager.Instance.chuPaiPanel.SetActive(false);
        PlayManager.Instance.ClearAllChuList();
        NetworkLoader.Instance.UpdateProperties("buyaoCnt", null);
        NetworkLoader.Instance.UpdateProperties("chuList", null);
        NetworkLoader.Instance.UpdateProperties("nowBig", null);
        NetworkLoader.Instance.UpdateProperties("Record", null);   //账本清空
        PlayManager.Instance.baodaoWin = false;

    }
    private void Update()
    {
        
        if(Time.time-lastRefresh>cdtime && PhotonNetwork.CurrentRoom!=null )
        {
            GetSeatList();
            if (NetworkLoader.Instance.CurrentStatus == GameProgress.FullPlayer)
            {
                GetSeatList();
            }
            if(PhotonNetwork.CurrentRoom.Players.Count>=4 && NetworkLoader.Instance.CurrentStatus == GameProgress.WaitForPlayer)
            {
                NetworkLoader.Instance.CurrentStatus = GameProgress.FullPlayer;
            }
            if(PhotonNetwork.CurrentRoom.Players.Count<4)
            {
                NetworkLoader.Instance.CurrentStatus = GameProgress.WaitForPlayer;
            }
            lastRefresh = Time.time;
        }
    }
    public void UpdateList()
    {
        for (int i = 0; i < nameList.childCount; i++)
        {
            Destroy(nameList.GetChild(i).gameObject);
        }

        Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
        int roomHostId = 999;
        foreach (var p in playerList)
        {
            GameObject newButton = Instantiate(playerNamePrefab, nameList.position, Quaternion.identity);
            newButton.GetComponentInChildren<Text>().text = p.Key.ToString()+":"+p.Value.NickName;
            newButton.transform.SetParent(nameList);
            if (p.Key < roomHostId)
                roomHostId = p.Key;
        }
        NetworkLoader.Instance.UpdateProperties("roomHost", roomHostId.ToString());
        
    }

    public void GetSeatList()
    {
        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["seatList"]);
    }

    

}
