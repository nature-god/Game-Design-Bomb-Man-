using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class MessageManager : NetworkBehaviour {

    public Text message;
    public GameObject[] Players;

    private float freshTime = 60.0f;            //场地木块刷新频率
    private float time = 0.0f;                  //场地木块刷新计时器
    public bool gameOver = false;              //游戏是否结束
    public int winner;                         //胜利者：0为玩家，1为boss

    private bool flag = true;                  //控制游戏结束锁

    // Use this for initialization
    void Start () {
        CmdMessage(false);
    }

    // Update is called once per frame
    void Update () {
        if (gameOver)
        {
            if (flag)
            {
                if (winner == 0)
                {
                    CmdSetMessage("Players Win!");
                }
                else
                {
                    CmdSetMessage("Boss Win!");
                }
                CmdIsCanControl();
                flag = false;
            }
            else
            {
                return;
            }
        }
        else
        {
            Players = GetPlayers();
            CmdbossWin();
        }
    }
    [Command]
    void CmdIsCanControl()
    {
        RpcIsCanControl();
    }

    [ClientRpc]
    void RpcIsCanControl()
    {
        GameObject.Find("localPlayer").GetComponent<Player>().isCanControl = false;
    }

    [Command]
    public void CmdSetMessage(string value)
    {
        RpcSetMessage(value);
    }
    [ClientRpc]
    public void RpcSetMessage(string value)
    {
        message.gameObject.SetActive(true);
        message.text = value;
    }

    [Command]
    public void CmdMessage(bool value)
    {
        RpcMessage(value);
    }
    [ClientRpc]
    public void RpcMessage(bool value)
    {
        message.gameObject.SetActive(value);
    }

    [Command]
    public void CmdbossWin()
    {
        if(isAllNull(Players))
        {
            return;
        }

        foreach (GameObject tmp in Players)
        {
            if (tmp != null)
            {
                if (tmp.GetComponent<Player>().status != Player.Status.dead)
                {
                    gameOver = false;
                    return;
                }
                else
                {
                    if (tmp.GetComponent<Player>().MedicalNums > 0)
                    {
                        gameOver = false;
                        return;
                    }
                }
            }
            else
            {
                if(Players.Length == 1)
                {
                    return;
                }
            }
        }
        winner = 1;
        gameOver = true;
    }

    private GameObject[] GetPlayers()
    {
        int num = 0;
        GameObject[] tmp = new GameObject[GameObject.FindGameObjectsWithTag("Player").Length];
        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (temp.GetComponent<Player>().playerNumber == 1)
            {
                tmp[num] = temp;
                num++;
            }
        }
        return tmp;
    }

    private void OnGUI()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        GUIStyle tmp = new GUIStyle();
        tmp.fontSize = 20;
        tmp.alignment = TextAnchor.MiddleCenter;
        tmp.normal.textColor = Color.black;
        GUI.Label(new Rect(50, 0, 100, 100), "下次地图刷新时间: " + ((int)(freshTime - time)).ToString(), tmp);
    }

    private bool isAllNull(GameObject[] tmp)
    {
        foreach(GameObject temp in tmp)
        {
            if(temp != null)
            {
                return false;
            }
        }
        return true;
    }
}
