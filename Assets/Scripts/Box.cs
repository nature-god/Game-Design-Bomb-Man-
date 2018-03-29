using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour {
    //木箱，被炸了会销毁，掉落物品
    public Transform AllBox;
    public Transform AllItem;
    public GameObject[] Items;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Explosion"))
        {
            int temp = Random.Range(0, 3);
            Instantiate(Items[temp], transform.position, transform.rotation, AllItem);
            Destroy(gameObject);
        }
    }
}
