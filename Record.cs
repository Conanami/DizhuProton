using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Record : MonoBehaviour
{
    public bool finishSend;
    public bool startSend;

    public GameObject oneLine;
    public GameObject playerNameLine;
    public Transform content;
    public Button nextGame;
    public Button closeBtn;
   
    private float refreshTime;
    private int lastRecordCnt;
    // Start is called before the first frame update
    void Start()
    {
        //SetupPlayerName();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-refreshTime>3)   //3秒刷新一次账本
        {
            ShowRecord();
            refreshTime = Time.time;
        }
    }

    void ShowRecord()
    {
        SetupPlayerName();
        if (NetworkLoader.Instance.GetProperties("Record") != null)
        {
            string recordStr = NetworkLoader.Instance.GetProperties("Record");
            List<int> recordList = NetworkLoader.Instance.Str2IntList(recordStr);
            if (lastRecordCnt < recordList.Count)
            {
                for(int k = 0;k<content.childCount;k++)
                {
                    GameObject obj = content.GetChild(k).gameObject;
                    obj.transform.SetParent(null);
                    Destroy(obj);
                }
                for (int i = 0; i < recordList.Count / 4; i++)
                {
                    GameObject newLine;
                    newLine = Instantiate(oneLine);
                    newLine.transform.SetParent(content);
                    for (int j = 0; j < 4; j++)
                    {
                        newLine.transform.GetChild(j).GetComponent<Text>().text = recordList[i * 4 + j].ToString();
                    }


                }
            }
            lastRecordCnt = recordList.Count;
        }
    }

    
    void SetupPlayerName()
    {
        if (NetworkLoader.Instance.GetProperties("seatList") != null && NetworkLoader.Instance.GetProperties("Record") != null)
        {
            List<int> seatList = PlayManager.Instance.seatList;
            for (int i = 0; i < seatList.Count; i++)
            {
                playerNameLine.transform.GetChild(i).GetComponent<Text>().text = PlayManager.Instance.nicknameList[i];
            }
        }
    }
    public void ShowButton(bool flag)
    {
        nextGame.gameObject.SetActive(flag);
        closeBtn.gameObject.SetActive(!flag);
    }
    public void Jizhang(int lordkey)
    {
        if (finishSend == false)
        {
            string tmpRecord = GetRecordStr(lordkey);
            if (NetworkLoader.Instance.GetProperties("Record") == null)
            {
                NetworkLoader.Instance.UpdateProperties("Record", tmpRecord);
            }
            else
            {
                string oldRecord = NetworkLoader.Instance.GetProperties("Record");
                List<int> recordList = NetworkLoader.Instance.Str2IntList(oldRecord);
                List<int> thisLine = NetworkLoader.Instance.Str2IntList(tmpRecord);
                string thisLineStr = "";
                for (int i = recordList.Count - 4; i < recordList.Count; i++)
                {
                    int balance = recordList[i] + thisLine[i - (recordList.Count - 4)];
                    thisLineStr = thisLineStr + "," + balance.ToString();
                }
                tmpRecord = oldRecord + thisLineStr;
                NetworkLoader.Instance.UpdateProperties("Record", tmpRecord);
            }
            finishSend = true;
        }
    }

    public string GetRecordStr(int lordkey)
    {
        string recordStr = "";
        if (NetworkLoader.Instance.GetProperties("betsize") != null)
        {
            List<int> seatList = PlayManager.Instance.seatList;
            int betsize = int.Parse(NetworkLoader.Instance.GetProperties("betsize"));
            

            for (int i = 0; i < seatList.Count; i++)
            {
                int money;
                if (PlayManager.Instance.dizhuWin)
                {
                    if (seatList[i] == lordkey)
                    {
                        money = betsize * 5 * 3;
                    }
                    else
                    {
                        money = -betsize * 5;
                    }

                }
                else
                {
                    if (betsize < 4)
                    {
                        if (seatList[i] == lordkey)
                        {
                            money = -betsize * 5 * 3;
                        }
                        else
                        {
                            money = betsize * 5;
                        }
                    }
                    else  //头撩地主输15
                    {
                        if (seatList[i] == lordkey)
                        {
                            money = -3 * 5 * 3;
                        }
                        else
                        {
                            money = 3 * 5;
                        }
                    }
                }
                if (i > 0)
                {
                    recordStr = recordStr + "," + money.ToString();
                }
                else
                {
                    recordStr = money.ToString();
                }
            }
        }
        return recordStr;
    }
    public void StartNext()
    {
        NetworkLoader.Instance.CurrentStatus = GameProgress.WaitForCard;
            //
        Time.timeScale = 1;
        transform.gameObject.SetActive(false);
        finishSend = false;
        
    }


}
