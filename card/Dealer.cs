using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Dealer:MonoBehaviour
{
    public static Dealer Instance;
    public GameObject cardPrefab;
    public List<Sprite> cardSprites;
    public Sprite backSprite;
    public List<GameObject> wholeSet = new List<GameObject>();
    public List<GameObject> afterDeal = new List<GameObject>();
    public Canvas canvas;
    public List<int> seatList = new List<int>();  //只记录玩家的key，名字不记，不重复记录
    private int setcount;
    public List<int> cardInHand = new List<int>();  //记录每张牌的状态，在底部，在谁手里

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public void CreateList()   //买牌，有牌就不用买
    {
        if (wholeSet.Count == 0)
        {
            for (int s = 0; s < 2; s++)
            {
                setcount = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 2; j < 17; j++)
                    {
                        if (j <= 14)
                        {
                            GameObject newCard = null;
                            if(j==2)
                                newCard = InstantiateCard(s, i, 15, setcount);
                            else
                                newCard = InstantiateCard(s, i, j,setcount);
                            setcount++;
                            wholeSet.Add(newCard);
                            newCard.transform.SetParent(transform);
                            //newCard.transform.localPosition = new Vector3(-150-600*s+(j+1)*25, -600+(i+1)*125, 0);
                        }
                        else
                        {
                            if (i < 1)
                            {
                                GameObject newCard = InstantiateCard(s, i, j+1,setcount);
                                setcount++;
                                wholeSet.Add(newCard);
                                newCard.transform.SetParent(transform);
                                //newCard.transform.localPosition = new Vector3(-150-600*s +(j + 1) * 25, -600, 0);
                            }
                        }

                    }
                }


            }
        }
       
        
    }

    public void CreateSeat()
    {
        CreateList();
        //GetSeatCard();   //得到一张抽位子的牌
    }


    
    private void ShowCardList(List<GameObject> set)
    {
        for (int i = 0; i < set.Count; i++)
        {
           set[i].transform.SetParent(null);
        }

        for (int i = 0; i < set.Count; i++)
        {

            int tmpY = i / 25;
            int tmpX = i % 25;
            set[i].transform.SetParent(canvas.transform);
            set[i].transform.localPosition = new Vector3(-600+tmpX*30, -500+tmpY*125, 0);

        }
    }

    private GameObject InstantiateCard(int setno,int color,int number,int setcount)
    {
        GameObject newCard = Instantiate(cardPrefab);
        newCard.GetComponent<Card>().setno = setno;
        newCard.GetComponent<Card>().color = color;
        newCard.GetComponent<Card>().number = number;
        newCard.GetComponent<Card>().SetSprite(cardSprites[setcount]);
        return newCard;
    }

    //洗牌发牌是最麻烦的
    public List<GameObject> FisherYatesShuffle(List<GameObject> list,List<int> seatlist)
    {
        List<GameObject> copy = new List<GameObject>();
        
        for (int i = 0; i < list.Count; i++)
        {
            copy.Add(list[i]);
        }
        List<GameObject> cache = new List<GameObject>();
        int currentIndex;
        while (cache.Count < list.Count)
        {
            currentIndex = UnityEngine.Random.Range(0, copy.Count);
            cache.Add(copy[currentIndex]);
            int originIndex = list.FindIndex(xxx => xxx.Equals(copy[currentIndex]));
            if (cache.Count <= 100)
            {
                list[originIndex].transform.GetComponent<Card>().owner = seatList[ (cache.Count-1)/ 25];
            }
            else
            {
                list[originIndex].transform.GetComponent<Card>().owner = 0; //底牌
            }
            copy.RemoveAt(currentIndex);
        }
        return cache;
    }

    public string GetCardOwnerList()
    {
        string rtstr = "";
        foreach(var obj in wholeSet)
        {
            rtstr = rtstr+obj.transform.GetComponent<Card>().owner.ToString() + ",";        
        }
        rtstr = rtstr.TrimEnd(',');
        return rtstr;
    }

    public void ShuffleAndDeal()
    {
        CreateList();
        //测试一下反复发牌的功能
        if (NetworkLoader.Instance.CurrentStatus == GameProgress.WaitForCard)
        {
            afterDeal = FisherYatesShuffle(wholeSet, seatList);
            //ShowCardList(afterDeal);
            Debug.Log(GetCardOwnerList());
            NetworkLoader.Instance.UpdateProperties("card", GetCardOwnerList());
            NetworkLoader.Instance.UpdateProperties("lord", null);
        }
    }

    
    //安排位置，把位置传给大家
    public void AssignSeat()
    {
        
        seatList.Clear();
        foreach (var p in PhotonNetwork.CurrentRoom.Players)
        {
            seatList.Add(p.Key);
        }
        seatList = ShuffleList(seatList);
        NetworkLoader.Instance.UpdateProperties("seatList", string.Join(",", seatList));
        
        NetworkLoader.Instance.CurrentStatus = GameProgress.SeatAssigned;
        NetworkLoader.Instance.UpdateProperties("gameProgress", GameProgress.SeatAssigned.ToString());
        NetworkLoader.Instance.TurnPlayer = UnityEngine.Random.Range(0, seatList.Count);
        NetworkLoader.Instance.UpdateProperties("restartList", string.Join(",", seatList));

    }
    public List<T> ShuffleList<T>(List<T> list)
    {
        List<T> copy = new List<T>();

        for (int i = 0; i < list.Count; i++)
        {
            copy.Add(list[i]);
        }
        List<T> cache = new List<T>();
        int currentIndex;
        while (cache.Count < list.Count)
        {
            currentIndex = UnityEngine.Random.Range(0, copy.Count);
            cache.Add(copy[currentIndex]);
            int originIndex = list.FindIndex(xxx => xxx.Equals(copy[currentIndex]));
            copy.RemoveAt(currentIndex);
        }
        return cache;

    }

    
}
