using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class GlobalStateManager : NetworkBehaviour {

    public bool ExitDoor = false;

    public Transform AllBoxs;
    public Transform AllItems;
    public GameObject[] StartPos;
    public Text Message;
    public GameObject SpeedShoes;
    public GameObject BombPower;
    public GameObject BombNums;
    public GameObject Box;
    public GameObject[] Players;

    private int BoxNum = 30;                    //场地中刷新Box的最大数目
    
    private GameObject[] Blocks;                //获取场地边界
    private GameObject[] Floor;                 //获取场地floor(地板块)

    private float freshTime = 60.0f;            //场地木块刷新频率
    private float time = 0.0f;                  //场地木块刷新计时器
    public bool gameOver = false;              //游戏是否结束
    public int winner;                         //胜利者：0为玩家，1为boss

    private bool flag = true;                  //控制游戏结束锁

    private void Awake()
    {
        Blocks = GameObject.FindGameObjectsWithTag("Block");
        Floor = GameObject.FindGameObjectsWithTag("Floor");
        Players = new GameObject[5];
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }
        else
        {
            if (time <= freshTime)
            {
                time += Time.deltaTime;
            }
            else
            {
                time = 0.0f;
                CmdBoxProduct(BoxNum);
            }
            Players = GetPlayers();
            CmdbossWin();
        }
    }

    /// <summary>
    /// 刷新场地内所有box
    /// </summary>
    /// <param name="num"></param>
    [Command]
    public void CmdBoxProduct(int num)
    {
        foreach (GameObject var in GameObject.FindGameObjectsWithTag("Item"))
        {
            Destroy(var);
        }
        foreach (GameObject var in GameObject.FindGameObjectsWithTag("Box"))
        {
            Destroy(var);
        }
        foreach (GameObject var in GameObject.FindGameObjectsWithTag("Bomb"))
        {
            var.GetComponent<Bomb>().producer.GetComponent<Player>().BombNums++;
            Destroy(var);
        }

        for (int i = 0; i < num; i++)
        {
            bool temp = true;
            int place = Random.Range(0, Floor.Length - 1);

            if(GameObject.FindGameObjectWithTag("Door")!=null)
            {
                if ((Floor[place].transform.position.x == GameObject.FindGameObjectWithTag("Door").transform.position.x)
                    && (Floor[place].transform.position.z == GameObject.FindGameObjectWithTag("Door").transform.position.z))
                {
                    temp = false;
                }
            }
            if(GameObject.FindGameObjectsWithTag("BossBox")!=null)
            {
                foreach (GameObject var in GameObject.FindGameObjectsWithTag("BossBox"))
                {
                    if ((Floor[place].transform.position.x == var.transform.position.x)
                        && (Floor[place].transform.position.z == var.transform.position.z))
                    {
                        temp = false;
                        break;
                    }
                }
            }

            foreach (GameObject var in Blocks)
            {
                if ((Floor[place].transform.position.x == var.transform.position.x)
                    && (Floor[place].transform.position.z == var.transform.position.z))
                {
                    temp = false;
                    break;
                }
            }

            if (temp)
            {
                foreach (GameObject var in GameObject.FindGameObjectsWithTag("Box"))
                {
                    if ((Floor[place].transform.position.x == var.transform.position.x)
                        && (Floor[place].transform.position.z == var.transform.position.z))
                    {
                        temp = false;
                        break;
                    }
                }
                if (temp)
                {
                    if (((StartPos[0].transform.position.x == Floor[place].transform.position.x) && (StartPos[0].transform.position.z == Floor[place].transform.position.z))
                        || ((StartPos[1].transform.position.x == Floor[place].transform.position.x) && (StartPos[1].transform.position.z == Floor[place].transform.position.z)))
                    {
                        continue;
                    }
                    else
                    {
                        Vector3 temPos = new Vector3(Floor[place].transform.position.x, 0.50f, Floor[place].transform.position.z);
                        var gtmp = (GameObject)Instantiate(Box, temPos, Floor[place].transform.rotation, AllBoxs);

                        if (!ExitDoor)
                        {
                            int s = Random.Range(0, 5);
                            if (s == 4)
                            {
                                gtmp.GetComponent<Box>().ExitDoor = true;
                                ExitDoor = true;
                            }
                        }

                        NetworkServer.Spawn(gtmp);
                    }
                }
                else
                {
                    temp = true;
                    continue;
                }
            }
            else
            {
                temp = true;
                continue;
            }
        }
    }

    public override void OnStartServer()
    {
        CmdBoxProduct(BoxNum);
    }

    [Command]
    public void CmdbossWin()
    {
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
                    if (tmp.GetComponent<Player>().MedicalNums != 0)
                    {
                        gameOver = false;
                        return;
                    }
                }
            }
            else
            {
                return;
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
        GUIStyle tmp = new GUIStyle();
        tmp.fontSize = 20;
        tmp.alignment = TextAnchor.MiddleCenter;
        tmp.normal.textColor = Color.black;
        GUI.Label(new Rect(50, 0, 100, 100), "下次地图刷新时间: "+((int)(freshTime - time)).ToString(),tmp);
    }
}
