using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusCanvasPosition : MonoBehaviour {

    private Quaternion rotation;

	// Use this for initialization
	void Start () {
        rotation = Quaternion.Euler(new Vector3(-270.0f, 90.0f, 90.0f));
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = rotation;
	}
}
