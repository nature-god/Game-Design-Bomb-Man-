using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GlobalStateManager : MonoBehaviour {
    public Transform AllBoxs;
    public Transform AllItems;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject[] StartPos;
    public Text Message;
    public GameObject SpeedShoes;
    public GameObject BombPower;
    public GameObject BombNums;
    public GameObject Box;
    public bool RandomProductItem = false;

    private bool gamebegin = false;
    public bool gameBegin
    {
        set
        {
            foreach(GameObject var in GameObject.FindGameObjectsWithTag("Player"))
            {
                var.GetComponent<Player>().isCanControl = value;
            }
            gamebegin = value;
        }
        get
        {
            return gamebegin;
        }
    }

    private GameObject[] Blocks;
    private GameObject[] Floor;
    private float time = 0.0f;
    private int deadPlayers = 0;
    private int deadPlayerNumber = -1;
    private void Start()
    {
        Blocks = GameObject.FindGameObjectsWithTag("Block");
        Floor = GameObject.FindGameObjectsWithTag("Floor");
        Message.gameObject.SetActive(false);
        Instantiate(Player1,StartPos[0].transform.position,StartPos[0].transform.rotation);
        Instantiate(Player2,StartPos[1].transform.position,StartPos[1].transform.rotation);
        gameBegin = true;
        BoxProduct(12);
    }

    private void Update()
    {
        if (!gameBegin)
            return;
        else
        {
            if(RandomProductItem)
            {
                if (time <= 4.0f)
                {
                    time += Time.deltaTime;
                }
                else
                {
                    time = 0.0f;
                    ItemProduct();
                }
            }
            else
            {
                return;
            }
        }
    }

    public void PlayerDied(int playerNumber)
    {
        deadPlayers++;

        if (deadPlayers == 1)
        {
            deadPlayerNumber = playerNumber;
            Invoke("CheckPlayersDeath", .3f);
        }
    }

    void CheckPlayersDeath()
    {
        if (deadPlayers == 1)
        { //Single dead player, he's the winner
            Message.gameObject.SetActive(true);

            if (deadPlayerNumber == 1)
            { //P1 dead, P2 is the winner
                Debug.Log("Player 2 is the winner!");
                Message.color = Color.blue;
                Message.text = "Player 2 is the winner!";
            }
            else
            { //P2 dead, P1 is the winner
                Debug.Log("Player 1 is the winner!");
                Message.color = Color.red;
                Message.text = "Player 1 is the winner!";
            }
        }
        else
        {  //Multiple dead players, it's a draw
            Message.gameObject.SetActive(true);

            Debug.Log("The game ended in a draw!");
            Message.text = "The game ended in a draw!";
        }
        gameBegin = false;

        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(2.0f);
        Setup();
    }

    public void Setup()
    {
        if(GameObject.Find("Player 1(Clone)") != null)
        {
            Destroy(GameObject.Find("Player 1(Clone)").gameObject);
        }
        if (GameObject.Find("Player 2(Clone)") != null)
        {
            Destroy(GameObject.Find("Player 2(Clone)").gameObject);
        }
        Instantiate(Player1, StartPos[0].transform.position, StartPos[0].transform.rotation);
        Instantiate(Player2, StartPos[1].transform.position, StartPos[1].transform.rotation);
        Message.gameObject.SetActive(false);
        deadPlayers = 0;
        deadPlayerNumber = -1;
        gameBegin = true;
        foreach(GameObject var in GameObject.FindGameObjectsWithTag("Item"))
        {
            Destroy(var);
        }
        foreach (GameObject var in GameObject.FindGameObjectsWithTag("Box"))
        {
            Destroy(var);
        }
        foreach (GameObject var in GameObject.FindGameObjectsWithTag("Bomb"))
        {
            Destroy(var);
        }
        time = 0.0f;
        BoxProduct(12);
    }

    public void ItemProduct()
    {
        int place = Random.Range(0, Floor.Length-1);
        foreach(GameObject var in Blocks)
        {
            if((Floor[place].transform.position.x == var.transform.position.x) && (Floor[place].transform.position.z == var.transform.position.z))
            {
                return;
            }
        }
        Vector3 temPos = new Vector3(Floor[place].transform.position.x, 0.70f, Floor[place].transform.position.z);
        int temp = Random.Range(1,4);
        if(temp == 1)
        {
            Instantiate(SpeedShoes,temPos,Floor[place].transform.rotation,AllItems);
        }
        else if(temp == 2)
        {
            Instantiate(BombPower, temPos, Floor[place].transform.rotation, AllItems);
        }
        else
        {
            Instantiate(BombNums, temPos, Floor[place].transform.rotation, AllItems);
        }
    }

    public void BoxProduct(int num)
    {
        for(int i = 0;i<num;i++)
        {
            bool temp = true;
            int place = Random.Range(0, Floor.Length - 1);
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
                if(temp)
                {
                    if (((StartPos[0].transform.position.x == Floor[place].transform.position.x) && (StartPos[0].transform.position.z == Floor[place].transform.position.z))
                        || ((StartPos[1].transform.position.x == Floor[place].transform.position.x) && (StartPos[1].transform.position.z == Floor[place].transform.position.z)))
                    {
                        continue;
                    }
                    else
                    {
                        Vector3 temPos = new Vector3(Floor[place].transform.position.x, 0.50f, Floor[place].transform.position.z);
                        Instantiate(Box, temPos, Floor[place].transform.rotation, AllBoxs);
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
}
