using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStatus : MonoBehaviour
{
    public int playerKey;
    public Text txtName;
    public Image imgStatus;
    public Text txtLeftCard;
    public GameObject chuList;
    public int leftCard;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupUI(string name,bool isTurn , bool isChuturn)
    {
        txtName.text = name;
        if(isTurn==true)
            imgStatus.color = new Color((230 / 255f), (27 / 255f), (245 / 255f), (70 / 255f));
        else
            imgStatus.color = new Color((255 / 255f), (255 / 255f), (255 / 255f), (70 / 255f));

        if(isChuturn == true)
            imgStatus.color = new Color((255 / 255f), (255 / 255f), (20 / 255f), (70 / 255f));
        
    }

    public void UpdateLeftCard()
    {
        txtLeftCard.text = this.leftCard.ToString();
    }
    public void SetTurn(bool isTurn)
    {
        if (isTurn == true)
            imgStatus.color = new Color((230 / 255f), (27 / 255f), (245 / 255f), (70 / 255f));
        else
            imgStatus.color = new Color((255 / 255f), (255 / 255f), (255 / 255f), (70 / 255f));
    }

    internal void SetupUI(bool isTurn, bool isChuturn)
    {
        if (isTurn == true)
            imgStatus.color = new Color((230 / 255f), (27 / 255f), (245 / 255f), (70 / 255f));
        else
            imgStatus.color = new Color((255 / 255f), (255 / 255f), (255 / 255f), (70 / 255f));

        if (isChuturn == true)
            imgStatus.color = new Color((255 / 255f), (255 / 255f), (20 / 255f), (70 / 255f));
    }
}
