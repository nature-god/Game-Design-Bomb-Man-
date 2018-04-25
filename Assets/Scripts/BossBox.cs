using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Explosion"))
        {
            Debug.Log("XXXXXXX");
            Destroy(gameObject, 0.1f);
        }
    }
}
