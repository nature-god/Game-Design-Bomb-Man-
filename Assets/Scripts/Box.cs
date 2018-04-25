using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Box : NetworkBehaviour {
    //木箱，被炸了会销毁，掉落物品
    public Transform AllBox;
    public Transform AllItem;
    public GameObject[] Items;
    public GameObject Door;
    public bool ExitDoor = false;
	// Use this for initialization
	void Start () {
        AllBox = GameObject.Find("Map/Boxs").transform;
        this.transform.SetParent(AllBox);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explosion")&&(!ExitDoor))
        {
            CmdOnRangdom();         
        }

        if (other.CompareTag("Explosion")&&(ExitDoor))
        {
            CmdDoor();
        }
    }


    [Command]
    private void CmdOnRangdom()
    {
        int temp = Random.Range(0, 5);
        AllItem = GameObject.Find("AllItems").transform;
        var tmp = Instantiate(Items[temp], transform.position, transform.rotation, AllItem);
        NetworkServer.Spawn(tmp);
        Destroy(gameObject, 0.1f);
    }
    [Command]
    private void CmdDoor()
    {
        AllItem = GameObject.Find("AllItems").transform;
        var tmp = Instantiate(Door, transform.position, transform.rotation, AllItem);
        NetworkServer.Spawn(tmp);
        Destroy(gameObject, 0.1f);
    }
}
