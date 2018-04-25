using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().playerNumber != 1)
            {
                other.GetComponent<Player>().status = Player.Status.giddy;
                Destroy(this.gameObject);
            }
        }
        if (other.CompareTag("Explosion"))
        {
            Destroy(this.gameObject);
        }
    }
}
