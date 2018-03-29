using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {
    private int bombPower;          //炸弹威力[范围1到4格]
    private float moveSpeed ;       //移动速度
    private int bombNums;           //可以释放的炸弹数量,释放一个减一，炸一个加一
    public bool isCanControl;
    
    public int BombPower{
        set { bombPower = value; }
        get { return bombPower; }
    }
    public float MoveSpeed
    {
        set { moveSpeed = value; }
        get { return moveSpeed; }
    }
    public int BombNums
    {
        set { bombNums = value;
            Debug.Log(bombNums);
        }
        get { return bombNums; }
    }

    [Range(1, 2)] 
    public int playerNumber = 1;        //区分玩家P1与P2
    public Transform BombPlace;
    public bool canDropBombs = true;    //是否可以下炸弹
    public bool canMove = true;         //是否是可移动区域
    public bool dead = false;           //该玩家是否死亡
    public GlobalStateManager GlobalManager;

    //Prefabs
    public GameObject bombPrefab;       //炸弹预制体

    //Cached components
    private Rigidbody rigidBody;        //刚体组件
    private Transform myTransform;      //移动组件
    private Animator animator;          //动画组件

    // Use this for initialization
    void Start() {
        //初始化
        bombNums = 2;
        bombPower = 2;
        MoveSpeed = 5.0f;

        GlobalManager = GameObject.Find("Global State Manager").GetComponent<GlobalStateManager>();
        rigidBody = GetComponent<Rigidbody>();
        myTransform = transform;
        animator = myTransform.Find("PlayerModel").GetComponent<Animator>();
        if(playerNumber == 1)
        {
            BombPlace = GameObject.Find("PlayerBombs/Player1").transform;
        }
        else
        {
            BombPlace = GameObject.Find("PlayerBombs/Player2").transform;
        }
    }

    // Update is called once per frame
    void Update() {
        if (!isCanControl)
            return;

        UpdateMovement();
    }

    private void UpdateMovement() {
        animator.SetBool("Walking", false);

        if (!canMove) { //Return if player can't move
            return;
        }

        //Depending on the player number, use different input for moving
        if (playerNumber == 1) {
            UpdatePlayer1Movement();
        }
        else {
            UpdatePlayer2Movement();
        }
    }

    /// <summary>
    /// Updates Player 1's movement and facing rotation using the WASD keys and drops bombs using Space
    /// </summary>
    private void UpdatePlayer1Movement() {
        if (Input.GetKey(KeyCode.W)) { //Up movement
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetBool("Walking",true);
        }

        if (Input.GetKey(KeyCode.A)) { //Left movement
            rigidBody.velocity = new Vector3(-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.S)) { //Down movement
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.D)) { //Right movement
            rigidBody.velocity = new Vector3(moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
            animator.SetBool("Walking", true);
        }

        if (canDropBombs && Input.GetKeyDown(KeyCode.Space)) { //Drop bomb
            DropBomb();
        }
    }

    /// <summary>
    /// Updates Player 2's movement and facing rotation using the arrow keys and drops bombs using Enter or Return
    /// </summary>
    private void UpdatePlayer2Movement() {
        if (Input.GetKey(KeyCode.UpArrow)) { //Up movement
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 0, 0);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.LeftArrow)) { //Left movement
            rigidBody.velocity = new Vector3(-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 270, 0);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.DownArrow)) { //Down movement
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler(0, 180, 0);
            animator.SetBool("Walking", true);
        }

        if (Input.GetKey(KeyCode.RightArrow)) { //Right movement
            rigidBody.velocity = new Vector3(moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler(0, 90, 0);
            animator.SetBool("Walking", true);
        }

        if (canDropBombs && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))) { //Drop Bomb. For Player 2's bombs, allow both the numeric enter as the return key or players without a numpad will be unable to drop bombs
            DropBomb();
        }
    }

    /// <summary>
    /// DropBomb 设置炸弹,BombNums减一;
    /// </summary>
    private void DropBomb() {
        if (bombPrefab && BombNums > 0) { //可以释放炸弹
            BombNums--;
            Instantiate(bombPrefab, 
                new Vector3(Mathf.RoundToInt(myTransform.position.x), bombPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z)), 
                bombPrefab.transform.rotation,BombPlace);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!dead && other.CompareTag("Explosion"))
        { //Not dead & hit by explosion
            Debug.Log("P" + playerNumber + " hit by explosion!");

            dead = true;
            GlobalManager.PlayerDied(playerNumber); //Notify global state manager that this player died
            Destroy(gameObject);
        }
    }
}
