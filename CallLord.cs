using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallLord : MonoBehaviour
{
    public Button noCall;
    public Button onlyOne;
    public Button plusOne;
    public Button plusTwo;
    public Text info;
    public bool called=false;   //是否叫过

    public float refreshInterval;
    private float lastRefresh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastRefresh > refreshInterval)
        {
            //UpdateInfo();
            UpdateButton();
            lastRefresh = Time.time;
        }
    }

    public void PressNoCall()
    {
        if(called==false)
        {
            PlayManager.Instance.bombLimit = 1;
            if (NetworkLoader.Instance.GetProperties("calledCount") != null)
            {
                if (int.Parse(NetworkLoader.Instance.GetProperties("calledCount")) >= 3)
                {
                    called = true;
                    string tmpCandidate = NetworkLoader.Instance.GetProperties("candidate");
                    NetworkLoader.Instance.UpdateProperties("lord", tmpCandidate);
                    transform.gameObject.SetActive(false);
                }
                else
                    NotifyPlayerTurn();
            }
            else
                NotifyPlayerTurn();
        }
            
        
    }
    public void PressOnlyOne()
    {
        if (called == false)
        {
            NetworkLoader.Instance.UpdateProperties("candidate", NetworkLoader.Instance.GetMyPlayerKey().ToString());
            //叫地主过程结束
            NetworkLoader.Instance.UpdateProperties("betsize", "1");

            if (NetworkLoader.Instance.GetProperties("calledCount") == null)
            {
                NotifyPlayerTurn();
            }
            else
            {
                if (int.Parse(NetworkLoader.Instance.GetProperties("calledCount")) >= 3)
                {
                    NetworkLoader.Instance.UpdateProperties("lord", NetworkLoader.Instance.GetMyPlayerKey().ToString());
                    transform.gameObject.SetActive(false);
                }
                else
                    NotifyPlayerTurn();
            }
        }
    }

    public void PressPlusOne()
    {
        if (called == false)
        {
            PlayManager.Instance.bombLimit = 2;
            NetworkLoader.Instance.UpdateProperties("candidate", NetworkLoader.Instance.GetMyPlayerKey().ToString());
            //叫地主过程结束
            NetworkLoader.Instance.UpdateProperties("betsize", "2");

            //更新本局大小（确定是1），1是最小
            if (NetworkLoader.Instance.GetProperties("calledCount") == null)
            {
                NotifyPlayerTurn();
            }
            else
            {
                if (int.Parse(NetworkLoader.Instance.GetProperties("calledCount")) >= 3)
                {
                    NetworkLoader.Instance.UpdateProperties("lord", NetworkLoader.Instance.GetMyPlayerKey().ToString());
                    transform.gameObject.SetActive(false);
                }
                else
                    NotifyPlayerTurn();
            }
        }

    }
    public void PressPlusTwo()
    {
        if (called == false)
        {
            called = true;
            NetworkLoader.Instance.UpdateProperties("candidate", NetworkLoader.Instance.GetMyPlayerKey().ToString());
            NetworkLoader.Instance.UpdateProperties("lord", NetworkLoader.Instance.GetMyPlayerKey().ToString());
            //叫地主过程结束
            if (NetworkLoader.Instance.GetProperties("calledCount") == null)
                NetworkLoader.Instance.UpdateProperties("betsize", "4");
            else
                NetworkLoader.Instance.UpdateProperties("betsize", "3");
            //NetworkLoader.Instance.CurrentStatus = GameProgress.PlayInProgress;
            transform.gameObject.SetActive(false);
        }
        //更新本局大小（确定是1），1是最小
        //NotifyPlayerTurn();
    }

    public void NotifyPlayerTurn()
    {
        called = true;
        int turn = int.Parse(NetworkLoader.Instance.GetProperties("turnPlayer"));
        int nextTurn = NetworkLoader.Instance.NextPlayer(turn);
        NetworkLoader.Instance.UpdateProperties("turnPlayer", nextTurn.ToString());
        UpdateCalledCount();
        transform.gameObject.SetActive(false);
        PlayManager.Instance.UpdateCallTurn();
    }
    public int UpdateCalledCount()
    {
        if (NetworkLoader.Instance.GetProperties("calledCount") == null)
        {
            NetworkLoader.Instance.UpdateProperties("calledCount", "1");
            return 1;
        }
        else
        {
            int calledCount = int.Parse(NetworkLoader.Instance.GetProperties("calledCount"));
            calledCount++;
            NetworkLoader.Instance.UpdateProperties("calledCount", calledCount.ToString());
            return calledCount;
        }
    }
    

    public void UpdateButton()
    {
        if(called==true)
        {
            transform.gameObject.SetActive(false);
            return;
        }
        if (NetworkLoader.Instance.GetProperties("betsize") == null)
        {
            if (NetworkLoader.Instance.GetProperties("calledCount") != null)
            {
                if (int.Parse(NetworkLoader.Instance.GetProperties("calledCount")) >= 3)
                {
                    noCall.gameObject.SetActive(false);
                    onlyOne.gameObject.SetActive(true);
                }
                else
                    onlyOne.gameObject.SetActive(false);
            }
            else
            {
                noCall.gameObject.SetActive(true);
                onlyOne.gameObject.SetActive(false);
            }
            
            plusOne.gameObject.SetActive(true);
            plusTwo.gameObject.SetActive(true);
        }
        if (NetworkLoader.Instance.GetProperties("betsize") == "1")
        {
            onlyOne.gameObject.SetActive(false);
            plusOne.gameObject.SetActive(true);
            plusTwo.gameObject.SetActive(true);
        }
        if (NetworkLoader.Instance.GetProperties("betsize") == "2")
        {

            onlyOne.gameObject.SetActive(false);
            plusOne.gameObject.SetActive(false);
            plusTwo.gameObject.SetActive(true);
        }
        
    }
}
