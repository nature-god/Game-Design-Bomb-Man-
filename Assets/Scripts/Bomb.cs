using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;

public class Bomb : MonoBehaviour
{
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    public GameObject producer;
    public LayerMask levelMask; // This LayerMask makes sure the rays cast to check for free spaces only hits the blocks in the level
    private bool exploded = false;
    private int power;
    // Use this for initialization
    void Start()
    {
        producer = GameObject.Find("localPlayer");          //获取player
        power = producer.GetComponent<Player>().BombPower;
        Invoke("Explode", 3f); //Call Explode in 3 seconds
    }

    public void Explode()
    {
        //Explosion sound
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        //Create a first explosion at the bomb position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        //For every direction, start a chain of explosions
        StartCoroutine(CreateExplosions(Vector3.forward,power));
        StartCoroutine(CreateExplosions(Vector3.right,power));
        StartCoroutine(CreateExplosions(Vector3.back,power));
        StartCoroutine(CreateExplosions(Vector3.left,power));

        GetComponent<MeshRenderer>().enabled = false; //Disable mesh
        exploded = true;
        transform.Find("Collider").gameObject.SetActive(false); //Disable the collider
        producer.GetComponent<Player>().BombNums++;
        Destroy(gameObject, .3f); //Destroy the actual bomb in 0.3 seconds, after all coroutines have finished       
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!exploded && other.CompareTag("Explosion"))
        { //If not exploded yet and this bomb is hit by an explosion...
            CancelInvoke("Explode"); //Cancel the already called Explode, else the bomb might explode twice 
            Explode(); //Finally, explode!
        }
    }

    private IEnumerator CreateExplosions(Vector3 direction, int power)
    {
        for (int i = 1; i < power; i++)
        { //The 3 here dictates how far the raycasts will check, in this case 3 tiles far
            RaycastHit hit; //Holds all information about what the raycast hits

            Physics.Raycast(transform.position + new Vector3(0, .5f, 0), direction, out hit, i, levelMask); //Raycast in the specified direction at i distance, because of the layer mask it'll only hit blocks, not players or bombs

            if (!hit.collider)
            { // Free space, make a new explosion
                var tmp = Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                NetworkServer.Spawn(tmp);
            }
            else
            { //Hit a block, stop spawning in this direction
                if (hit.collider.CompareTag("Box")|| hit.collider.CompareTag("BossBox"))
                {
                    var tmp = Instantiate(explosionPrefab, transform.position + (i * direction), explosionPrefab.transform.rotation);
                    tmp.transform.GetChild(0).gameObject.SetActive(false);
                    NetworkServer.Spawn(tmp);
                }
                break;
            }
            yield return new WaitForSeconds(.05f); //Wait 50 milliseconds before checking the next location
        }

    }
}