using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class NameUI : MonoBehaviourPunCallbacks
{
    public InputField playerName;
    public Button enterGame;
    //public GameObject nameList;
    private void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            enterGame.interactable = true;
            enterGame.onClick.AddListener(JoinGame);
        }
    }
    
    public void JoinGame()
    {
        //PhotonNetwork.NickName = playerName.text;
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };

        string[] keys = { "users", "conditions" };

        options.CustomRoomPropertiesForLobby = keys;

        PhotonNetwork.JoinOrCreateRoom("Room", options, default);
        PhotonNetwork.NickName = playerName.text;
        Debug.Log(PhotonNetwork.NickName + " Join room.");
        this.gameObject.SetActive(false);
        //nameList.SetActive(true);

        
    }

    
}
