using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public List<GameObject> posList = new List<GameObject>();
    public float refreshInterval;
    private float lastRefresh;
    public Text info;
    public Dealer dealer;
    public GameObject callLord;
    public GameObject handCard;
    public GameObject diPai;
    public GameObject chuPaiPanel;
    public GameObject recordPanel;

    public Button liPai;
    
    public bool IsSeated;      //安排位子
    public bool GotHandCard;   //拿到牌
    public bool CalledLord;    //是否叫过地主
    public bool ChupaiFinish;  //是否出牌完毕

    public int localTurn;

    public bool GotDipai = false;
    public List<int> seatList;
    public Dictionary<int, Player> playerList;
    public List<int> bigList;
    public static PlayManager Instance;
    public bool dizhuWin;
    public bool baodaoWin;   //是否报到

    public int bombLimit = 99;  //可以用的炸弹数量
    public List<string> nicknameList;
    public void ShowRecordPanel()
    {
        if (recordPanel.activeSelf == false)
        {
            recordPanel.SetActive(true);
            recordPanel.GetComponent<Record>().ShowButton(false);
        }

    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Update()
    {
        if (Time.time - lastRefresh > refreshInterval)
        {
            if (NetworkLoader.Instance.CurrentStatus != GameProgress.PlayInProgress &&
                NetworkLoader.Instance.CurrentStatus != GameProgress.CallLord)
            {
                //Debug.Log("PlayerManager:" + NetworkLoader.Instance.GetProperties("gameProgress"));
                if (NetworkLoader.Instance.CurrentStatus == GameProgress.WaitForPlayer && PhotonNetwork.CurrentRoom != null)
                {
                    RandomArrangeSeat();
                }
                if (NetworkLoader.Instance.CurrentStatus == GameProgress.FullPlayer)
                {
                    if (NetworkLoader.Instance.GetProperties("roomHost") == NetworkLoader.Instance.GetMyPlayerKey().ToString())
                    {
                        dealer.AssignSeat();
                    }

                }
                if (NetworkLoader.Instance.GetProperties("seatList") != null && IsSeated == false
                    && NetworkLoader.Instance.CurrentStatus != GameProgress.WaitForPlayer)
                {
                    Debug.Log("Show all in seats");
                    ArrangeSeat();
                }

                if (NetworkLoader.Instance.CurrentStatus == GameProgress.WaitForCard )
                {
                    InitCallLord();
                    if (NetworkLoader.Instance.GetProperties("card") == null &&
                        NetworkLoader.Instance.GetProperties("roomHost") == NetworkLoader.Instance.GetMyPlayerKey().ToString())
                    {
                        InitNetworkCallLord();
                        dealer.ShuffleAndDeal();
                    }
                    
                }
                
            }
            else
            {
                if (NetworkLoader.Instance.CurrentStatus == GameProgress.CallLord)
                {


                    if (NetworkLoader.Instance.myTurn())
                        callLord.gameObject.SetActive(true);
                    else
                        callLord.gameObject.SetActive(false);
                    UpdateInfo();
                    UpdateCallTurn();
                }
                else
                {
                    if (NetworkLoader.Instance.CurrentStatus == GameProgress.PlayInProgress)
                    {
                        callLord.gameObject.SetActive(false);
                        UpdateInfo();
                        liPai.gameObject.SetActive(true);
                        int remoteTurn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
                        if (NetworkLoader.Instance.myTurn())
                        {
                            if (!ChupaiFinish)
                            {
                                chuPaiPanel.SetActive(true);
                                chuPaiPanel.GetComponent<ChuPai>().UpdateButton();
                            }
                        }
                        else
                        {
                            chuPaiPanel.SetActive(false);
                            ChupaiFinish = false;
                        }
                    }
                }
            }
            lastRefresh = Time.time;

        }
    }

    
    public void UpdateInfo()
    {
        if (NetworkLoader.Instance.CurrentStatus == GameProgress.PlayInProgress)
        {
            //Debug.Log("正式打牌要更新的信息");
            if (NetworkLoader.Instance.GetProperties("turnPlayer") != null)
            {
                try
                {
                    int chuTurn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
                    info.text = "轮到" + PlayManager.Instance.GetPlayerName(chuTurn) + "出牌";
                    UpdateChuTurn();
                    UpdateChuList();
                }
                catch(Exception e)
                {
                    Debug.Log(e.ToString());
                }
                
            }
        }
        else
        {
            if (NetworkLoader.Instance.GetProperties("lord") == null)
            {
                if (NetworkLoader.Instance.GetProperties("candidate") == null)
                {

                    int callTurn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
                    Debug.Log("叫地主轮到" + callTurn.ToString());
                    info.text = "轮到" + PlayManager.Instance.GetPlayerName(callTurn) + "叫地主";
                }
                else
                {
                    info.text = playerList[int.Parse(NetworkLoader.Instance.GetProperties("candidate"))].NickName
                        + "叫了" + NetworkLoader.Instance.GetProperties("betsize") + "档";
                }
            }
            else
            {
                info.text = playerList[int.Parse(NetworkLoader.Instance.GetProperties("lord"))].NickName
                        + "叫了" + NetworkLoader.Instance.GetProperties("betsize") + "档，分配底牌";
                //地主产生后
                UpdateLordSeat();
                if (GotDipai == false)
                {
                    GetDipai();
                    NetworkLoader.Instance.CurrentStatus = GameProgress.PlayInProgress;
                }
            }
        }

    }
    public void HideCallLord()
    {
        callLord.gameObject.SetActive(false);
    }
    public void RandomArrangeSeat()
    {
        for (int i = 0; i < posList.Count; i++)
        {
            posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI("空位", false, false);
        }

        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        playerList = NetworkLoader.Instance.GetPlayerList();

        string strSeatlist = NetworkLoader.Instance.GetProperties("seatList");
        int cnt = 0;
        foreach (var p in playerList)
        {
            posList[cnt].transform.GetComponentInChildren<PlayerStatus>().SetupUI(p.Value.NickName, false, false);
            cnt++;
        }
        info.text = "正在等人到齐，还没有摸位子，随便坐的";
    }

    public string GetPlayerName(int indexOfSeatList)
    {

        return playerList[seatList[indexOfSeatList]].NickName;
    }
    public void ArrangeSeat()
    {

        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        playerList = NetworkLoader.Instance.GetPlayerList();
        string strSeatlist = NetworkLoader.Instance.GetProperties("seatList");
        seatList = NetworkLoader.Instance.Str2IntList(strSeatlist);
        int offset = seatList.FindIndex(xxx => xxx.Equals(myPlayerKey));
        nicknameList.Clear();
        for (int i = 0; i < seatList.Count; i++)
        {
            Player p = null;
            playerList.TryGetValue(seatList[(offset + i) % seatList.Count], out p);
            if ((offset + i) % seatList.Count == 0)
            {
                posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI(p.NickName, true, false);
                posList[i].transform.GetComponentInChildren<PlayerStatus>().playerKey = p.ActorNumber;
            }
            else
            {
                posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI(p.NickName, false, false);
                posList[i].transform.GetComponentInChildren<PlayerStatus>().playerKey = p.ActorNumber;
            }
            nicknameList.Add(playerList[seatList[i]].NickName);
            
        }
        info.text = "人已到齐，位置分配完毕。";
        NetworkLoader.Instance.CurrentStatus = GameProgress.WaitForCard;
        IsSeated = true;

    }
    public void UpdateCallTurn()
    {
        if (NetworkLoader.Instance.GetProperties("turnPlayer") != null)
        {
            int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
            int turn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
            int offset = seatList.FindIndex(xxx => xxx.Equals(myPlayerKey));
            for (int i = 0; i < seatList.Count; i++)
            {
                if (i == turn)
                    posList[(i - offset+4) % seatList.Count].transform.GetComponentInChildren<PlayerStatus>().SetupUI(true, false);
                else
                    posList[(i - offset+4) % seatList.Count].transform.GetComponentInChildren<PlayerStatus>().SetupUI(false, false);
            }
        }
          
    }

    public void UpdateLordSeat()
    {
        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        int offset = seatList.FindIndex(xxx => xxx.Equals(myPlayerKey));
        int lordKey = int.Parse(NetworkLoader.Instance.GetProperties("lord"));
        for (int i = 0; i < seatList.Count; i++)
        {
            Player p = null;
            playerList.TryGetValue(seatList[(offset + i) % seatList.Count], out p);
            if (p.ActorNumber == lordKey)
            {
                posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI("地主:"+p.NickName, false, true);
                posList[i].transform.GetComponentInChildren<PlayerStatus>().leftCard = 33;
            }
            else
            {
                posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI(p.NickName, false, false);
                posList[i].transform.GetComponentInChildren<PlayerStatus>().leftCard = 25;
            }
        }
    }

    public void GetDipai()
    {
        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        int lordKey = int.Parse(NetworkLoader.Instance.GetProperties("lord"));
        Debug.Log("Getdipai:" + myPlayerKey.ToString() + "," + lordKey.ToString());
        if (myPlayerKey == lordKey)
        {
            Debug.Log("Change Parent");
            for (int i = 0; i < diPai.transform.childCount; i++)
            {
                diPai.transform.GetChild(i).GetComponent<Card>().ShowFront();
                diPai.transform.GetChild(i).GetComponent<Card>().owner = lordKey;
            }
            handCard.GetComponent<HandCard>().UpdateMyCards();
            SendCardcntList(lordKey);
            SendTurnPlayer(lordKey);
            bombLimit = 99;
        }
        else
        {
            for (int i = 0; i < diPai.transform.childCount; i++)
            {
                diPai.transform.GetChild(i).GetComponent<Card>().ShowFront();
                diPai.transform.GetChild(i).GetComponent<Card>().owner = lordKey;
            }
            handCard.GetComponent<HandCard>().UpdateCardList();
        }
        GotDipai = true;
        InitPlay();
    }

    private void SendCardcntList(int lordKey)
    {
        string cardCountList="";
        for (int i = 0; i < seatList.Count; i++)
        {
            if(i>0)
            {
                cardCountList = cardCountList + ",";
            }
            if(seatList[i]!=lordKey)
            {
                cardCountList = cardCountList + "25";
            }
            else
            {
                cardCountList = cardCountList + "33";
            }
            
        }
        NetworkLoader.Instance.UpdateProperties("CardCountList", cardCountList);
        
    }
    public void UpdateCardcntList(int cnt)
    {
        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        string cardCntListStr = NetworkLoader.Instance.GetProperties("CardCountList");
        List<int> cardCntList = NetworkLoader.Instance.Str2IntList(cardCntListStr);
        for (int i = 0; i < cardCntList.Count; i++)
        {
            if(seatList[i]==myPlayerKey)
            {
                cardCntList[i] -= cnt;
            }
        }
        cardCntListStr = "";
        for (int i = 0; i < seatList.Count; i++)
        {
            if (i > 0)
            {
                cardCntListStr = cardCntListStr + ",";
            }
            cardCntListStr = cardCntListStr + cardCntList[i].ToString();
        }
        NetworkLoader.Instance.UpdateProperties("CardCountList", cardCntListStr);
    }
    void SendTurnPlayer(int lordKey)
    {
        for (int i = 0; i < seatList.Count; i++)
        {
            if(seatList[i]==lordKey)
            {
                NetworkLoader.Instance.UpdateProperties("turnPlayer", i.ToString());
                return;
            }

        }
    }

    public void UpdateChuTurn()
    {
        if (NetworkLoader.Instance.GetProperties("turnPlayer") != null && NetworkLoader.Instance.GetProperties("lord")!=null)
        {
            int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
            int lordKey = int.Parse(NetworkLoader.Instance.GetProperties("lord"));
            int turn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
            
            string cardCntListStr = NetworkLoader.Instance.GetProperties("CardCountList");
            List<int> cardCntList = NetworkLoader.Instance.Str2IntList(cardCntListStr);
            int offset = seatList.FindIndex(xxx => xxx.Equals(myPlayerKey));

            for (int i = 0; i < seatList.Count; i++)
            {
                if (posList[i].GetComponentInChildren<PlayerStatus>().playerKey == seatList[turn])
                {
                    posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI(false, true);
                    
                }
                else
                {
                    posList[i].transform.GetComponentInChildren<PlayerStatus>().SetupUI(false, false);
                }
                
                posList[i].transform.GetComponentInChildren<PlayerStatus>().leftCard = cardCntList[(i + offset) % seatList.Count];
                posList[i].transform.GetComponentInChildren<PlayerStatus>().UpdateLeftCard();
                if(cardCntList[(i + offset) % seatList.Count]==0)
                {
                    if(lordKey==seatList[(i + offset) % seatList.Count])
                    {
                        info.text = "地主赢了！";
                        dizhuWin = true;
                        GameResult(lordKey);
                    }
                    else
                    {
                        info.text = "农民赢了！";
                        dizhuWin = false;
                        GameResult(lordKey);
                    }
                }
                
            }
            
        }
    }

    public void InitCallLord()
    {
        GotHandCard = false;
        GotDipai = false;
        callLord.SetActive(false);
        callLord.GetComponent<CallLord>().called = false;
        chuPaiPanel.SetActive(false);
        bombLimit = 99;
    }

    public void InitNetworkCallLord()
    {
        NetworkLoader.Instance.UpdateProperties("lord", null);
        NetworkLoader.Instance.UpdateProperties("betsize", null);
        NetworkLoader.Instance.UpdateProperties("candidate", null);
        NetworkLoader.Instance.UpdateProperties("calledCount", null);
    }

    public void InitPlay()
    {
        ChupaiFinish = false;
        ClearAllChuList();
        NetworkLoader.Instance.UpdateProperties("buyaoCnt", null);
        NetworkLoader.Instance.UpdateProperties("chuList", null);
        NetworkLoader.Instance.UpdateProperties("nowBig", null);
        baodaoWin = false;
    }

    public void GameResult(int lordkey)
    {
        InitPlay();
        //Time.timeScale = 0;
        NetworkLoader.Instance.CurrentStatus = GameProgress.EndGame;
        //
        ClearAllChuList();
        handCard.GetComponent<HandCard>().ResetCards();
        NetworkLoader.Instance.UpdateProperties("card", null);
        NetworkLoader.Instance.UpdateProperties("restartList", null);
        
        if (NetworkLoader.Instance.GetProperties("roomHost") == NetworkLoader.Instance.GetMyPlayerKey().ToString())
        {
            recordPanel.GetComponent<Record>().Jizhang(lordkey);
        }
        recordPanel.SetActive(true);
        recordPanel.GetComponent<Record>().ShowButton(true);
    }
    
    public void BuChu()
    {
        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        ClearChuList(posList[0]);
        posList[0].GetComponentInChildren<ChuList>().GetComponent<Text>().text ="不出";
        NetworkLoader.Instance.UpdateProperties("nowBuyao", myPlayerKey.ToString());
    }
    public void ChuPai()
    {
        NetworkLoader.Instance.UpdateProperties("buyaoCnt", "0");
        posList[0].GetComponentInChildren<ChuList>().GetComponent<Text>().text = "";
        ClearChuList(posList[0]);
        string chuStr = "";
        int myPlayerKey = NetworkLoader.Instance.GetMyPlayerKey();
        int cnt=0;
        for (int i = 0; i < handCard.transform.childCount; i++)
        {
            if(handCard.transform.GetChild(i).GetComponent<Card>().readyGo)
            {
                handCard.transform.GetChild(i).GetComponent<Card>().owner = -1;
                int cardIndex = dealer.wholeSet.FindIndex(xxx => xxx.Equals(handCard.transform.GetChild(i).gameObject));
                if (chuStr == "")
                    chuStr = cardIndex.ToString();
                else
                    chuStr = chuStr +","+ cardIndex.ToString();
                cnt++;
            }
        }
        handCard.GetComponent<HandCard>().UpdateChuList();
        if (baodaoWin)
            cnt = posList[0].GetComponentInChildren<PlayerStatus>().leftCard;


        posList[0].GetComponentInChildren<PlayerStatus>().leftCard -= cnt;
        posList[0].GetComponentInChildren<PlayerStatus>().UpdateLeftCard();
        UpdateCardcntList(cnt);
        NetworkLoader.Instance.UpdateProperties("nowBig", myPlayerKey.ToString());
        NetworkLoader.Instance.UpdateProperties("chuList", chuStr);
        
    }
    public void UpdateChuList()
    {
        if(NetworkLoader.Instance.GetProperties("nowBig")==null && ChupaiFinish==false)
        {
            ClearAllChuList();
            //清空所有
        }
        else
        {
            if (NetworkLoader.Instance.GetProperties("buyaoCnt") == "3")
            {
                ClearAllChuList();
                NetworkLoader.Instance.UpdateProperties("nowBig", null);
                NetworkLoader.Instance.UpdateProperties("chuList", null);
                NetworkLoader.Instance.UpdateProperties("nowBuyao", null);
            }
            if (NetworkLoader.Instance.CurrentStatus == GameProgress.PlayInProgress)
            {
                int nowBig;
                if (NetworkLoader.Instance.GetProperties("nowBig") != null)
                    nowBig = int.Parse(NetworkLoader.Instance.GetProperties("nowBig"));
                else
                    nowBig = -1;
                int nowBuyao;
                if (NetworkLoader.Instance.GetProperties("nowBuyao") == null)
                    nowBuyao = -1;
                else
                    nowBuyao = int.Parse(NetworkLoader.Instance.GetProperties("nowBuyao"));

                for (int i = 1; i < posList.Count; i++)
                {
                    
                    if (posList[i].GetComponentInChildren<PlayerStatus>().playerKey == nowBig)
                    {
                        ClearChuList(posList[i]);
                        posList[i].GetComponentInChildren<ChuList>().GetComponent<Text>().text ="";
                        string strChuList = NetworkLoader.Instance.GetProperties("chuList");
                        bigList = NetworkLoader.Instance.Str2IntList(strChuList);
                        for (int j = 0; j < bigList.Count; j++)
                        {
                            dealer.wholeSet[bigList[j]].transform.SetParent(posList[i].GetComponentInChildren<ChuList>().transform);
                        }
                        switch (i)
                        {
                            case 1:
                                handCard.GetComponent<HandCard>().Lipai(posList[i].GetComponentInChildren<ChuList>().transform, -30*bigList.Count);
                                break;
                            case 2:
                                handCard.GetComponent<HandCard>().Lipai(posList[i].GetComponentInChildren<ChuList>().transform, -15 * bigList.Count);
                                break;
                            case 3:
                                handCard.GetComponent<HandCard>().Lipai(posList[i].GetComponentInChildren<ChuList>().transform, 0);
                                break;
                        }
                        

                    }
                    if(posList[i].GetComponentInChildren<PlayerStatus>().playerKey == nowBuyao)
                    {
                        ClearChuList(posList[i]);
                        posList[i].GetComponentInChildren<ChuList>().GetComponent<Text>().text ="不出";
                    }
                }
            }
        }
    }
    public void ClearChuList(GameObject pos)
    {
        for (int j = 0; j < pos.GetComponentInChildren<ChuList>().transform.childCount; j++)
        {
            pos.GetComponentInChildren<ChuList>().transform.GetChild(j).GetComponent<Card>().owner = -2;
        }
        for(int j=0;j<dealer.wholeSet.Count;j++)
        {
            if(dealer.wholeSet[j].GetComponent<Card>().owner==-2)
            {
                dealer.wholeSet[j].transform.SetParent(dealer.transform);
                dealer.wholeSet[j].GetComponent<Card>().owner = 0;
            }
        }

    }

    public void ClearAllChuList()
    {
        for (int i = 0; i < posList.Count; i++)
        {
            ClearChuList(posList[i]);
            posList[i].GetComponentInChildren<ChuList>().GetComponent<Text>().text="";
        }
    }

    
}
