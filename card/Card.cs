using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Card : MonoBehaviour
{
    public int setno;       //哪副牌
    public int color;       //什么花色
    public int number;      //几
    public int owner;    //0表示已经打出
    public bool readyGo=false;  //如果为true就是要打出了
    public GameObject front;
    public GameObject back;
    
    public void SetSprite(Sprite sprite)
    {
        front.GetComponent<Image>().sprite = sprite;
        ShowFront();
    }

    public void ShowBack()
    {
        back.SetActive(true);
    }
    
    
    public void ShowFront()
    {
        back.SetActive(false);
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(PrepareGo);
    }

    void PrepareGo()
    {
        if (owner > 0)
        {
            int readyCnt = GetReadyCnt();
            readyGo = !readyGo;
            if (readyGo)
            {
                if (transform.localPosition.y == 0)
                {
                    transform.localPosition += new Vector3(0, 50, 0);
                }
                MoreGo(true,readyCnt);
            }
            else
            {
                if (transform.localPosition.y == 50)
                    transform.localPosition -= new Vector3(0, 50, 0);
                //MoreGo(false,readyCnt);
            }
        }
    }
    public void MoreGo(bool flag,int readyCnt = 0)
    {
        if (NetworkLoader.Instance.GetProperties("chuList") != null)
        {
            List<int> bigNumList = ToNumList(NetworkLoader.Instance.GetProperties("chuList"));
            int[] nowBigs = bigNumList.ToArray();
            nowBigs = bubbleSort(nowBigs);
            string bigType = CheckType.Instance.getCardType(nowBigs);
            string[] bigTypeList = bigType.Split(':');
            if(bigTypeList[0]=="BOMB" && flag)
            {
                SameNumber(flag);
            }
            if(bigTypeList[0]=="FEI2" && flag)
            {
                SameNumber(flag);
            }
            if(bigTypeList[0]=="FEI0" && flag)
            {
                SameNumber(flag);
            }
            if(bigTypeList[0]=="DOUB" && flag)
            {
                OneMore(flag);
            }
            if(int.Parse(bigTypeList[1])>=2 && int.Parse(bigTypeList[2])==14)
            {
                SameNumber(flag);
            }
            if (int.Parse(bigTypeList[1])==1 && int.Parse(bigTypeList[2]) == 15)
            {
                SameNumber(flag);
            }
            if(bigTypeList[0] == "LIST" && int.Parse(bigTypeList[2]) == 14)
            {
                SameNumber(flag);
            }
            //如果现在最大的炸弹，而我点了自己的炸弹，我的炸弹比他的大，那么就一起出

        }
    }
    public void SameNumber(bool flag)
    {
        int cnt = PlayManager.Instance.handCard.GetComponent<HandCard>().transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            if (PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().number == number)
            {
                PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().TogetherGo(flag);
            }
        }
    }

    public int GetReadyCnt()  //得到已经准备出的牌数
    {
        int cnt = PlayManager.Instance.handCard.GetComponent<HandCard>().transform.childCount;
        int rt = 0;
        for (int i = 0; i < cnt; i++)
        {
            if (PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().readyGo)
            {
                rt++;             
            }
        }
        return rt;
    }
    public void OneMore(bool flag)
    {
        int cnt = PlayManager.Instance.handCard.GetComponent<HandCard>().transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            if (PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().number == number
                && PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>()!=this)
            {
                PlayManager.Instance.handCard.GetComponent<HandCard>().transform.GetChild(i).GetComponent<Card>().TogetherGo(flag);
                return;
            }
        }
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

    public void TogetherGo(bool toGo)
    {
        if (owner > 0)
        {
            readyGo = toGo;

            if (readyGo)
            {
                if (transform.localPosition.y == 0)
                {
                    transform.localPosition += new Vector3(0, 50, 0);
                }

            }
            else
            {
                if (transform.localPosition.y == 50)
                    transform.localPosition -= new Vector3(0, 50, 0);
            }
        }
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
}

