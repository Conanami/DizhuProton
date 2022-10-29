using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HandCard : MonoBehaviour
{
    public float cdtime;
    private float lastRefresh;

    public Dealer dealer;
    public Text info;
    private List<int> handCardList = new List<int>();
    public Transform dipai;
    public Transform chuList;
    public Button lipai;
    private int CardWidth=30;
    private void Update()
    {
        if (Time.time - lastRefresh > cdtime)
        {
            GetMycards();
            lastRefresh = Time.time;
        }

    }

    public void GetMycards()
    {
        if (NetworkLoader.Instance.GetProperties("card") != null && NetworkLoader.Instance.CurrentStatus == GameProgress.WaitForCard 
            && PlayManager.Instance.GotHandCard==false)
        {
            dealer.CreateList();
            ShowMyCards();
            PlayManager.Instance.GotHandCard = true;
            NetworkLoader.Instance.CurrentStatus = GameProgress.CallLord;
            
        }
    }


    public void ClearCards()
    {
        foreach (var c in transform.GetComponentsInChildren<Card>())
        {
            c.gameObject.transform.SetParent(dealer.transform);
            c.gameObject.transform.localPosition = Vector3.zero;
            c.ShowFront();
        }
        foreach (var c in dipai.GetComponentsInChildren<Card>())
        {
            c.gameObject.transform.SetParent(dealer.transform);
            c.gameObject.transform.localPosition = Vector3.zero;
            c.ShowFront();
        }
    }

    public void ResetCards()
    {
        for (int i = 0; i < dealer.wholeSet.Count; i++)
        {
            dealer.wholeSet[i].transform.SetParent(dealer.transform);
            dealer.wholeSet[i].transform.localPosition = Vector3.zero;
            dealer.wholeSet[i].transform.GetComponent<Card>().owner = 0;
            dealer.wholeSet[i].transform.GetComponent<Card>().readyGo = false;
        }

    }

    public void UpdateMyCards()
    {
        int playerKey = NetworkLoader.Instance.GetMyPlayerKey();
        int cnt = 0;
        for(int i=0;i<dealer.wholeSet.Count;i++)
        {
            if (dealer.wholeSet[i].transform.GetComponent<Card>().owner == playerKey)
            {
                dealer.wholeSet[i].transform.SetParent(dealer.transform);
                dealer.wholeSet[i].transform.GetComponent<Card>().ShowFront();
                dealer.wholeSet[i].transform.SetParent(transform);
                dealer.wholeSet[i].transform.localPosition = new Vector3(-15*CardWidth + cnt * CardWidth, 0, 0);
                cnt++;
            }
            else
            {
                dealer.wholeSet[i].transform.SetParent(dealer.transform);
                dealer.wholeSet[i].transform.localPosition = Vector3.zero;
            }
        }
    }
    
    public void UpdateCardList()
    {
        int playerKey = NetworkLoader.Instance.GetMyPlayerKey();
        int cnt = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<Card>().owner == playerKey)
            {
                //transform.GetChild(i).SetParent(dealer.transform);
                //transform.GetChild(i).GetComponent<Card>().ShowFront();
                //transform.GetChild(i).SetParent(transform);
                transform.GetChild(i).localPosition = new Vector3(-15 * CardWidth + cnt * CardWidth, 0, 0);
                cnt++;
            }
            
        }
        for (int i = 0; i < dealer.wholeSet.Count; i++)
        {
            if (dealer.wholeSet[i].transform.GetComponent<Card>().owner != playerKey)
            {
                dealer.wholeSet[i].transform.SetParent(dealer.transform);
                dealer.wholeSet[i].transform.GetComponent<Card>().ShowFront();
                dealer.wholeSet[i].transform.localPosition = Vector3.zero;
            }
        }
    }

    public void UpdateChuList()
    {
        int playerKey = NetworkLoader.Instance.GetMyPlayerKey();
        int cnt = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Card>().owner == playerKey)
            {
                //transform.GetChild(i).SetParent(dealer.transform);
                //transform.GetChild(i).GetComponent<Card>().ShowFront();
                //transform.GetChild(i).SetParent(transform);
                transform.GetChild(i).localPosition = new Vector3(-15 * CardWidth + cnt * CardWidth, 0, 0);
                cnt++;
            }

        }
        cnt = 0;
        for (int i = 0; i < dealer.wholeSet.Count; i++)
        {
            if (dealer.wholeSet[i].transform.GetComponent<Card>().owner == -1)
            {
                dealer.wholeSet[i].transform.SetParent(chuList);
                //dealer.wholeSet[i].transform.GetComponent<Card>().ShowFront();
                dealer.wholeSet[i].transform.localPosition = new Vector3(cnt * CardWidth, 0, 0);
                dealer.wholeSet[i].transform.GetComponent<Card>().owner = 0;
                cnt++;
            }
        }
        Lipai(chuList,-15*cnt,35);   //出牌位置调整
    }

    public void ShowMyCards()  //这是同步的结果
    {
        int playerKey = NetworkLoader.Instance.GetMyPlayerKey();
        info.text = playerKey.ToString();
        string ownerlist = NetworkLoader.Instance.GetProperties("card");
        handCardList = NetworkLoader.Instance.Str2IntList(ownerlist);
        SetWholeset(handCardList);
        int cnt = 0;
        int diCnt = 0;
        for (int i = 0; i < handCardList.Count ; i++)
        {
            if(dealer.wholeSet[i].transform.GetComponent<Card>().owner==playerKey)
            {
                dealer.wholeSet[i].transform.SetParent(transform);
                dealer.wholeSet[i].transform.GetComponent<Card>().ShowFront();
                dealer.wholeSet[i].transform.localPosition = new Vector3(-11*CardWidth + cnt * CardWidth, 0, 0);
                cnt++;
            }
            else
            {
                if(dealer.wholeSet[i].transform.GetComponent<Card>().owner==0)
                {
                    dealer.wholeSet[i].transform.GetComponent<Card>().ShowBack();
                    dealer.wholeSet[i].transform.SetParent(dipai);
                    dealer.wholeSet[i].transform.localPosition = new Vector3(-2*CardWidth + diCnt * CardWidth, 0, 0);
                    diCnt++;
                }
                
            }
            
        }
        Lipai(transform,-450);
        lipai.gameObject.SetActive(true);
    }
    public void ManualLipai() //手工理牌
    {
        //把标记为
        List<GameObject> tmplist = new List<GameObject>();
        List<GameObject> manuallist = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Card>().readyGo)
            {
                transform.GetChild(i).GetComponent<Card>().readyGo = false;
                manuallist.Add(transform.GetChild(i).gameObject);
            }
            else
                tmplist.Add(transform.GetChild(i).gameObject);
        }
        if(manuallist.Count>0)
        {
            for (int i = 0; i < tmplist.Count; i++)
            {
                manuallist.Add(tmplist[i]);
            }
        }
        ShowCards(manuallist,transform,-450);
    }

    public void LipaiOnClick()
    {
        List<GameObject> tmplist = new List<GameObject>();
        List<GameObject> manuallist = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Card>().readyGo)
                manuallist.Add(transform.GetChild(i).gameObject);
            else
                tmplist.Add(transform.GetChild(i).gameObject);
        }
        if (manuallist.Count > 0)
        {
            ManualLipai();
        }
        else
        {
            Lipai(transform,-15*CardWidth);
        }
    }
    public void Lipai(Transform transform,int leftStart,int y=0)
    {
        List<GameObject> tmplist = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            tmplist.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < tmplist.Count-1; i++)
        {
            for (int j = 0; j < tmplist.Count-i-1; j++)
            {
                if(tmplist[j].GetComponent<Card>().number<tmplist[j+1].GetComponent<Card>().number)
                {
                    GameObject tmp = tmplist[j];
                    tmplist[j] = tmplist[j + 1];
                    tmplist[j + 1] = tmp;
                }
            }

        }
        ShowCards(tmplist,transform,leftStart,y);
        
    }


    public void ShowCards(List<GameObject> tmplist,Transform transform,int leftStart,int y=0)
    {
        for (int i = 0; i < tmplist.Count; i++)
        {
            tmplist[i].transform.SetParent(dealer.transform);
            tmplist[i].transform.GetComponent<Card>().ShowFront();
            tmplist[i].transform.SetParent(transform);
            tmplist[i].transform.localPosition = new Vector3(leftStart + i * CardWidth, y, 0);
        }
    }

    //把发牌结果同步给所有人
    void SetWholeset(List<int> ownerlist)
    {
        for (int i = 0; i < ownerlist.Count; i++)
        {
            dealer.wholeSet[i].transform.GetComponent<Card>().owner = ownerlist[i];
        }
    }
    
}
