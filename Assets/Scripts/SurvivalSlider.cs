using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class SurvivalSlider : NetworkBehaviour {

    [SyncVar (hook ="OnchangeValue")]
    public float processValue = 0.0f;

    [SyncVar (hook ="OnchangeProcessOver")]
    public bool processOver = false;

    void OnchangeValue(float value)
    {
        processValue = value;
        slider.value = processValue;
    }
    void OnchangeProcessOver(bool value)
    {
        processOver = value;
    }


    public Slider slider;
    public Text text;

	// Use this for initialization
	void Start () {
        //slider = this.GetComponent<Slider>();
        //text = this.transform.GetChild(0).GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }
        if ((processValue < slider.maxValue) && (Input.GetKey(KeyCode.J)))
        {
            processValue += Time.deltaTime;
            slider.value = processValue;
            CmdSetSLider(processValue);
        }
        if ((processValue < slider.maxValue) && !(Input.GetKey(KeyCode.J)))
        {
            processValue = 0.0f;
            slider.value = processValue;
            CmdSetSLider(processValue);
        }
        if (slider.value >= slider.maxValue)
        {
            processOver = true;
            CmdSetProcessValue(true);
        }
    }

    [Command]
    void CmdSetSLider(float value)
    {
        RpcSetSlider(value);
    }
    [ClientRpc]
    void RpcSetSlider(float value)
    {
        slider.value = value;
    }
    [Command]
    void CmdSetProcessValue(bool value)
    {
        RpcSetProcessValue(value);
    }
    [ClientRpc]
    void RpcSetProcessValue(bool value)
    {
        processOver = value;
    }
}
