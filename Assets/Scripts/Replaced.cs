using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replaced : MonoBehaviour {
    public GameObject ParentObject;
    public GameObject Prafabs;

	// Use this for initialization
	void Start () {
		
        for(int i = 0;i<ParentObject.transform.childCount;i++)
        {
            Instantiate(Prafabs, ParentObject.transform.GetChild(i)).transform.localPosition = new Vector3(0,-0.5f,0);
        }
	}

}
