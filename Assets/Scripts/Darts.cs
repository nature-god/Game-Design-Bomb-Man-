using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Darts : MonoBehaviour {

    public float speed = 10.0f;
    private Vector3 forward;

	// Use this for initialization
	void Start () {
        forward = this.transform.up;

    }

    // Update is called once per frame
    void FixedUpdate () {
        this.transform.position += speed * forward * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            return;
        }
        else
        {
            if (other.CompareTag("BombCollider"))
            {
                Debug.Log("BOmbandfeibiao");
                //炸弹的collider是炸弹物体下的子物体
                other.transform.parent.GetComponent<Bomb>().Explode();
                //CmdExplodee(other);
            }
            Destroy(this.gameObject);

        }

    }
    //[Command]
    //void CmdExplodee(Collider var)
    //{
    //    RpcExplode(var);
    //}
    //[ClientRpc]
    //void RpcExplode(Collider var)
    //{
    //    var.transform.parent.GetComponent<Bomb>().CancelInvoke();

    //    var.transform.parent.GetComponent<Bomb>().Invoke("Explode",0f);
    //}
    
}
