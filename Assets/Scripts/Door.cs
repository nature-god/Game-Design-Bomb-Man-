using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    //终点

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.GetComponent<Player>().playerNumber == 1)
            {
                GameObject.Find("Global State Manager").GetComponent<GlobalStateManager>().gameOver = true;
                GameObject.Find("MessageManager").GetComponent<MessageManager>().gameOver = true;
                GameObject.Find("MessageManager").GetComponent<MessageManager>().winner = 0;


                Debug.Log("Player win!");
            }
        }
    }
}
