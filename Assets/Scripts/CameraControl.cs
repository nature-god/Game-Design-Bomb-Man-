using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    //To be continue

    private Transform _transform;
    private Transform _localPlayer;

    public Vector3 tmp;

    public float south = -2;
    public float north = 2;
    public float west = -1.6f;
    public float east = 0.8f;
    private float ftmp;
    private bool flag = false;

    // Use this for initialization
    void Start () {
        _transform = this.transform;
        ftmp = 12.4f;

        _localPlayer = GameObject.Find("localPlayer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(_localPlayer == null)
        {
            try
            {
                _localPlayer = GameObject.Find("localPlayer").transform;
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        _transform.position = new Vector3(_localPlayer.position.x, ftmp, _localPlayer.position.z-5);
        //if (_localPlayer == null)
        //{
        //    _localPlayer = GameObject.Find("localPlayer").transform;
        //    tmp = _transform.position - _localPlayer.position;
        //}
        //else
        //{
        //    if (_transform.position.z >= south)
        //    {
        //        Vector3 mn = (_localPlayer.position + tmp);
        //        mn.z = south;
        //        _transform.position = mn;
        //        flag = true;
        //    }
        //    if (_transform.position.z <= north)
        //    {
        //        Vector3 mn = (_localPlayer.position + tmp);
        //        mn.z = north;
        //        _transform.position = mn;
        //        flag = true;
        //    }
        //    if (_transform.position.x >= east)
        //    {
        //        Vector3 mn = (_localPlayer.position + tmp);
        //        mn.x = east;
        //        _transform.position = mn;
        //        flag = true;
        //    }
        //    if (_transform.position.x <= west)
        //    {
        //        Vector3 mn = (_localPlayer.position + tmp);
        //        mn.x = west;
        //        _transform.position = mn;
        //        flag = true;
        //    }
        //    if (!flag)
        //    {
        //        _transform.position = (_localPlayer.position + tmp);
        //        flag = false;
        //    }
        //}
    }
}
