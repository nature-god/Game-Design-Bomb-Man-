using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Item : NetworkBehaviour {
    //Item base Class
    public Transform AllItem;


    public enum ITEM_TYPE
    {
        PowerBottle,
        SpeedShoes,
        MoreBombs,
        MedicalBox,
        ElectricWeapon
    };

    public ITEM_TYPE itemType;
	// Use this for initialization
	void Start () {
        AllItem = GameObject.Find("AllItems").transform;
        this.transform.SetParent(AllItem);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explosion"))
        {
            Destroy(this.gameObject);
        }
        
        if (other.tag == "Player")
        {
            if (other.name != "localPlayer")
            {

            }
            else
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
                    case ITEM_TYPE.MedicalBox:
                        {
                            if (other.GetComponent<Player>().playerNumber != 1)
                            {
                                break;
                            }
                            else
                            {
                                other.GetComponent<Player>().MedicalNums++;
                                break;
                            }

                        }
                    case ITEM_TYPE.ElectricWeapon:
                        {
                            if (other.GetComponent<Player>().playerNumber != 1)
                            {
                                break;
                            }
                            else
                            {
                                other.GetComponent<Player>().EleWeaponNums++;
                                break;
                            }
                        }
                }
            }
            Destroy(this.gameObject);
        }
    }
}
