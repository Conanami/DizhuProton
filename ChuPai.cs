using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChuPai : MonoBehaviour
{
    public Text info;
    public Button buyaoBtn;
    public Button chupaiBtn;
    public Text buchuInfo;
    public string nowType;
    public string bigType;
    public bool finished;
    // Start is called before the first frame update
    public void OnClickBuchu()
    {
        if (finished == false)
        {
            finished = true;
            if (NetworkLoader.Instance.GetProperties("buyaoCnt") == null)
            {
                NetworkLoader.Instance.UpdateProperties("buyaoCnt", "1");
            }
            else
            {
                int buyaoCnt = int.Parse(NetworkLoader.Instance.GetProperties("buyaoCnt"));
                buyaoCnt++;
                NetworkLoader.Instance.UpdateProperties("buyaoCnt", buyaoCnt.ToString());
            }
            PlayManager.Instance.BuChu();
            NotifyPlayerTurn();
        }
    }

    public void OnClickChu()
    {
        if (finished == false)
        {
            string canStr = CanChu();
            if (canStr != "不符合规则")
            {
                string[] paiType = canStr.Split(':');
                if (paiType[0] == "BOMB" && PlayManager.Instance.bombLimit <= 0)
                {
                    info.text = "不能用蛋了";
                }
                else
                {
                    finished = true;
                    //info.text = "出牌完毕";
                    if (paiType[0] == "BOMB")
                        PlayManager.Instance.bombLimit--;
                    if (paiType[0] == "BOMB" && int.Parse(paiType[1]) >= 7)
                        PlayManager.Instance.baodaoWin = true;

                    info.text = "还能用" + PlayManager.Instance.bombLimit.ToString() + "把蛋";
                    GameObject chupos = PlayManager.Instance.posList[0].GetComponentInChildren<ChuList>().gameObject;
                    PlayManager.Instance.ChuPai();
                    NotifyPlayerTurn();
                }
            }
            else
            {
                info.text = "还能用" + PlayManager.Instance.bombLimit.ToString() + "把蛋";
            }
        }
    }

    public string CanChu()
    {
        int cnt = PlayManager.Instance.handCard.GetComponent<HandCard>().transform.childCount;
        //List<GameObject> chuCardList = new List<GameObject>();
        List<int> chuNumList = new List<int>();
        for (int i = 0; i < cnt; i++)
        {
            if (PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().readyGo)
            {
                //chuCardList.Add(PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).gameObject);
                chuNumList.Add(PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().number);
            }
        }
        int[] nums = chuNumList.ToArray();
        nums = bubbleSort(nums);
        string[] paiType;
        string[] oldType;
        Debug.Log(NetworkLoader.Instance.GetProperties("chuList"));
        Debug.Log("NowBig----"+NetworkLoader.Instance.GetProperties("nowBig"));
        Debug.Log("mine:" + string.Join(",", nums));
        //如果前面有大的，大的不是自己，那么必须比前面大
        if (NetworkLoader.Instance.GetProperties("nowBig")==null)
        {
                
            string tmp = CheckType.Instance.getCardType(nums);

            if (tmp.Trim() != "")
            {
                paiType = tmp.Split(':');
                return tmp;
            }
            else
                return "不符合规则";
                
        }
        else
        {
            Debug.Log(NetworkLoader.Instance.GetProperties("chuList"));

            if (NetworkLoader.Instance.GetProperties("chuList") != null)
            {
                List<int> bigNumList = ToNumList(NetworkLoader.Instance.GetProperties("chuList"));
                Debug.Log(string.Join(",", bigNumList.ToArray()));
                int[] nowBigs=bigNumList.ToArray();
                nowBigs = bubbleSort(nowBigs);
                nowType = CheckType.Instance.getCardType(nums);
                bigType = CheckType.Instance.getCardType(nowBigs);
                if (nowType.Trim() != "")
                {
                    Debug.Log(nowType);
                    Debug.Log(bigType);
                    paiType = nowType.Split(':');
                    oldType = bigType.Split(':');
                    if(paiType[0]==oldType[0] && paiType[1]==oldType[1] && int.Parse(paiType[2]) > int.Parse(oldType[2]))
                    {
                        
                        return nowType;
                    }
                    else
                    {
                        if(oldType[0]!="BOMB" && paiType[0]=="BOMB")
                        {
                            
                            return nowType;
                        }
                        if(oldType[0]=="BOMB" && paiType[0]=="BOMB")
                        {
                            if(int.Parse(oldType[1])<int.Parse(paiType[1]))
                            {
                                return nowType;
                            }
                        }
                        
                    }
                }
            }
            
        }
        return "不符合规则";
    }
    public List<int> ToNumList(string chuListStr)
    {
        List<int> SetIndexList = NetworkLoader.Instance.Str2IntList(chuListStr);
        List<int> numList = new List<int>();
        for (int i = 0; i < SetIndexList.Count; i++)
        {
            numList.Add(PlayManager.Instance.dealer.wholeSet[SetIndexList[i]].GetComponent<Card>().number);
        }
        return numList;
    }
    public void NotifyPlayerTurn()
    {
        transform.gameObject.SetActive(false);
        PlayManager.Instance.ChupaiFinish = true;
        int turn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
        int nextTurn = NetworkLoader.Instance.NextPlayer(turn);
        NetworkLoader.Instance.UpdateProperties("turnPlayer", nextTurn.ToString());
        //Todo:表现轮到谁出牌
        PlayManager.Instance.UpdateChuTurn();
        //finished = false;

    }
    public int[] bubbleSort(int[] nums)
    {
        for (int i = 0; i < nums.Length - 1; i++)
        {
            //第二层循环，根据循环次数，最大的一个数肯定在末尾，故不需要再比较后面的数，因为已经进行好排序了，所以这一层循环次数要减少之前已经排好序的个数再减一
            for (int j = 0; j < nums.Length - 1 - i; j++)
            {
                //int temp1 = array[j];//可注释，我是用于观察值
                //int temp2 = array[j + 1];//可注释，我是用于观察值
                //进行比较，如果当前数比后一个数大，那么就交换位置，确保最大的数移动到后面去
                if (nums[j] > nums[j + 1])
                {
                    //将当前值和比它小的值进行交换
                    int temp = nums[j];
                    nums[j] = nums[j + 1];
                    nums[j + 1] = temp;
                }
            }
        }
        return nums;
    }
    public void UpdateButton()
    {
        if (NetworkLoader.Instance.CurrentStatus == GameProgress.PlayInProgress)
        {
            if (NetworkLoader.Instance.GetProperties("buyaoCnt") == null
                && NetworkLoader.Instance.GetProperties("nowBig") == null)
            {
                PlayManager.Instance.InitPlay();
                buyaoBtn.gameObject.SetActive(false);
            }
            else
            {
                if (NetworkLoader.Instance.GetProperties("buyaoCnt") == "3")
                {
                    buyaoBtn.gameObject.SetActive(false);
                    PlayManager.Instance.InitPlay();
                }
                else
                    buyaoBtn.gameObject.SetActive(true);
            }
        }
        finished = false;
    }
}
