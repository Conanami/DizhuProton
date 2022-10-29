using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class UIManager : MonoBehaviour
{
    private float lastRefreshTime;
    public float refreshInterval;
    public Button SeatBtn;
    public Button DealBtn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastRefreshTime>refreshInterval)
        {
            string seatlist = NetworkLoader.Instance.GetProperties("seatlist");
            Dictionary<int, Player> playerList = NetworkLoader.Instance.GetPlayerList();
            if (seatlist!=null && playerList.Count==4)
            {
                SeatBtn.gameObject.SetActive(false);
            }
            
            lastRefreshTime = Time.time;
        }
        
    }
}
