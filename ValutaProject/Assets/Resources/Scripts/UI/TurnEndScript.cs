using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnEndScript : MonoBehaviour {

    //public GameObject player;
    public Text suncosttext;
    public Text mooncosttext;
    private int suncost = 0;
    private int mooncost = 0;
    //private Transform playerTrans;

    private int playerNumber = 0;
    void Start()
    {
        //playerTrans = player.GetComponent<Transform>();
        suncosttext.text = "suncost :" + suncost ;
        mooncosttext.text = "mooncost :" + mooncost;
    }
    public void OnClick()
    {
        if(playerNumber == 0)
        {
            playerNumber = 1;
            mooncost += 2;
            mooncosttext.text = "mooncost :" + mooncost;
        }
        else if(playerNumber == 1)
        {
            playerNumber = 0;
            suncost += 1;
            suncosttext.text = "suncost :" + suncost;
        }
        
        
        Debug.Log("turnend");
    }
    }
