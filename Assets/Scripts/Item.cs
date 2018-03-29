using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    //Item base Class

    public enum ITEM_TYPE
    {
        PowerBottle,
        SpeedShoes,
        MoreBombs
    };

    public ITEM_TYPE itemType;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Player")
        {
            switch (itemType)
            {
                case ITEM_TYPE.MoreBombs:
                    {
                        other.GetComponent<Player>().BombNums++;
                        break;
                    }
                case ITEM_TYPE.PowerBottle:
                    {
                        other.GetComponent<Player>().BombPower++;
                        break;
                    }
                case ITEM_TYPE.SpeedShoes:
                    {
                        other.GetComponent<Player>().MoveSpeed += 1.0f;
                        break;
                    }
            }
            Destroy(this.gameObject);
        }
    }
}
