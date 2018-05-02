using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Player : NetworkBehaviour {

    public enum Role
    {
        player,
        boss1,
        boss2,
        boss3
    }

    public enum Status
    {
        normal,                      //正常状态
        giddy,                       //眩晕状态
        opposite,                    //控制相反状态
        slow,                        //减速状态
        dead,                        //濒死状态
        help                         //救助他人状态
    };

    private float boss1_small_skill_time = 2.0f;
    private float boss1_big_skill_time = 20.0f;
    private float boss2_small_skill_time = 2.0f;
    private float boss2_big_skill_time = 20.0f;
    private float boss3_small_skill_time = 2.0f;
    private float boss3_big_skill_time = 20.0f;

    [SyncVar (hook= "walkStatusChange")]
    public bool walkFlag = false;

    private Status _status = Status.normal;
    public Status status
    {
        set {
            _status = value;
            CmdSetStatus(_status);
        }
        get { return _status; }
    }
    [Command]
    void CmdSetStatus(Status value)
    {
        RpcSetStatus(value);
    }
    [ClientRpc]
    void RpcSetStatus(Status value)
    {
        _status = value;
    }

    public Role role;
    public Image iStatus;
    public GameObject slider;
    public Text tMedicalNums;
    public Text tElectricWeaponNums;
    public Text tBombNums;
    public Text tBombPower;
    public Text tSpeedShoes;

    private int bombPower;          //炸弹威力[范围1到4格]
    private float moveSpeed ;       //移动速度
    private int bombNums;           //可以释放的炸弹数量,释放一个减一，炸一个加一

    private Image Icon1;            //图标1：对应玩家的医疗包，boss的小技能

    public GameObject helpedFriend;

    [SyncVar (hook ="SetMedicalNums")]
    public int medicalNums;        //可用医疗包数量
    private void SetMedicalNums(int value)
    {
        medicalNums = value;
    }

    [Command]
    private void CmdMedical(int value)
    {
        medicalNums = value;
    }

    private Image Icon2;            //图标2：对应玩家的电击陷阱，boss的大技能
    private int eleWeaponNums;      //可用电击陷阱数量

    public Transform[] Hand;



    private float opposite_forward = 1;   //反向前进
    private int opposite_rotate = 0;    //反向旋转
    public bool isCanControl;
    private float giddyTime = 0.0f;     //眩晕时间
    private float oppositeTime = 0.0f;  //反向操作持续时间
    private float slowTime = 0.0f;      //减速持续时间
    private float smallSkillTime = 0.0f;//小技能冷却时间
    private bool flagSmall = false;      //是否可以使用小技能
    private float bigSkillTime = 0.0f;  //大技能冷却时间
    private bool flagBig = false;        //是否可以使用大技能

    public int BombPower{
        set {
            bombPower = value;
            if(isLocalPlayer)
            {
                tBombPower.text = bombPower.ToString();
            }
        }
        get { return bombPower; }
    }
    public float MoveSpeed
    {
        set {
            moveSpeed = value;
            if (isLocalPlayer)
            {
                tSpeedShoes.text = ((int)moveSpeed).ToString();
            }
        }
        get { return moveSpeed; }
    }
    public int BombNums
    {
        set { bombNums = value;
            if (isLocalPlayer)
            {
                tBombNums.text = bombNums.ToString();
            }
        }
        get { return bombNums; }
    }
    public int MedicalNums
    {
        set
        {
            medicalNums = value;
            CmdMedical(medicalNums);
            if (isLocalPlayer)
            { 
               tMedicalNums.text = medicalNums.ToString();
            }
        }
        get { return medicalNums; }
    }
    public int EleWeaponNums
    {
        set {
            eleWeaponNums = value;
            if (isLocalPlayer)
            {
                tElectricWeaponNums.text = eleWeaponNums.ToString();
            }
        }
        get { return eleWeaponNums; }
    }

    [SyncVar (hook ="OnChangePlayerNumber")]
    [Range(1, 4)] 
    public int playerNumber = 1;        //区分普通玩家与boss

    private void OnChangePlayerNumber(int value)
    {
        Debug.Log("PSD" + value);
        playerNumber = value;
        switch(playerNumber)
        {
            case 1:
                {
                    role = Role.player;
                    break;
                }
            case 2:
                {
                    role = Role.boss1;
                    break;
                }
            case 3:
                {
                    role = Role.boss2;
                    break;
                }
            case 4:
                {
                    role = Role.boss3;
                    break;
                }
        }
    }
    
    [Command]
    private void CmdPlayerNumber(int value)
    {
        playerNumber = value;
    }

    public Transform BombPlace;
    public bool canDropBombs = true;    //是否可以下炸弹
    public bool canMove = true;         //是否是可移动区域
    public bool dead = false;           //该玩家是否死亡

   
    public GameObject bombPrefab;       //炸弹预制体
    public GameObject eleWeaponPrefab;        //电击陷阱
    public GameObject boxPrefab;              //物块预制体
    public GameObject dartsPrefab;            //飞镖预制体
    public GameObject bananaPrefab;           //香蕉皮预制体
    //public GlobalStateManager GlobalManager;

    //Cached components
    private Rigidbody rigidBody;        //刚体组件
    private Transform myTransform;      //移动组件
    private Animator animator;          //动画组件

    void Awake()
    {
        Debug.Log("Awake");
        rigidBody = GetComponent<Rigidbody>();
        myTransform = this.transform;
        animator = myTransform.Find("PlayerModel/Player").GetComponent<Animator>();
        tMedicalNums = GameObject.Find("Canvas/MedicalItems/Text").GetComponent<Text>();
        tElectricWeaponNums = GameObject.Find("Canvas/ElectricWeaponItems/Text").GetComponent<Text>();
        tBombNums = GameObject.Find("Canvas/BombNums/Text").GetComponent<Text>();
        tBombPower = GameObject.Find("Canvas/BombPower/Text").GetComponent<Text>();
        tSpeedShoes = GameObject.Find("Canvas/SpeedShoes/Text").GetComponent<Text>();
        Icon1 = GameObject.Find("Canvas/MedicalItems").GetComponent<Image>();
        Icon2 = GameObject.Find("Canvas/ElectricWeaponItems").GetComponent<Image>();
    }

    public override void OnStartLocalPlayer()
    {
        GameObject.Find("Canvas/Message").SetActive(false);
        playerNumber = PlayerPrefs.GetInt("player");
        Debug.Log(playerNumber);
        this.transform.name = "localPlayer";
        isCanControl = true;

        CmdPlayerNumber(playerNumber);
        #region player
        //普通玩家
        //初始炸弹数量：2个；
        //初始移速：10；
        //炸弹初始威力：2；


        if (playerNumber == 1)
        {
            SetSkillIcons("ElectricWeapon",Icon1);
            SetSkillIcons("medical", Icon2);
            BombNums = 2;
            BombPower = 2;
            MoveSpeed = 5.0f;
            EleWeaponNums = 1;
            MedicalNums = 1;
            role = Role.player;
            //this.transform.name = "localPlayer";
        }
        #endregion
        #region Boss1
        //Boss1:
        //初始炸弹数量：6个；
        //初始移速：8；
        //炸弹初始威力：3；
        //技能：   
        //     1.放置障碍物（可炸毁，无掉落物品）（冷却时间：2秒）
        //     2.主动刷新场地内的物块（冷却时间：20秒）
        else if (playerNumber == 2)
        {
            SetSkillIcons("small", Icon1);
            SetSkillIcons("Big", Icon2);
            BombNums = 6;
            BombPower = 3;
            MoveSpeed = 8.0f;
            EleWeaponNums = 1;
            MedicalNums = 1;
            role = Role.boss1;
            //this.transform.name = "Boss1";
        }
        #endregion
        #region Boss2
        //Boss2
        //初始炸弹数量：2个；
        //初始移速：15；
        //炸弹初始威力：1；                
        //技能：
        //     1.飞镖（飞镖击中炸弹会立马引爆炸弹）（冷却时间：5秒）
        //     2.旋舞（同时向四个方向发射飞镖）（冷却时间：20秒）；
        else if (playerNumber == 3)
        {
            SetSkillIcons("small", Icon1);
            SetSkillIcons("Big", Icon2);
            BombNums = 5;
            BombPower = 4;
            MoveSpeed = 5.0f;
            EleWeaponNums = 1;
            MedicalNums = 1;
            role = Role.boss2;
            //this.transform.name = "Boss2";
        }
        #endregion
        #region Boss3
        //Boss3
        //初始炸弹数量：4个；
        //初始移速：10；
        //炸弹初始威力：2；                
        //技能：
        //      1.香蕉皮（在地上放置一个香蕉皮，玩家踩中速度降低5点，持续10秒）（冷却时间：5秒）
        //      2.两极反转（游戏中所有玩家控制反向，持续5秒）  （冷却时间：30秒）
        else if (playerNumber == 4)
        {
            SetSkillIcons("small", Icon1);
            SetSkillIcons("Big", Icon2);
            BombNums = 5;
            BombPower = 4;
            MoveSpeed = 5.0f;
            EleWeaponNums = 1;
            MedicalNums = 1;
            role = Role.boss3;
            //this.transform.name = "Boss3";
        }
        #endregion
        else
        {
            Debug.Log("Role Error!");
        }
    }



    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer)
        {
            return;
        }
        if(!isCanControl)
        {
            return;
        }
        if (playerNumber == 1)
        {
            //player
            SetModel(1, 2, 3);
            switch (status)
            {
                case Status.normal:
                    {
                        opposite_forward = 1;
                        opposite_rotate = 0;
                        //iStatus.color = new Color(255, 255, 255, 0);
                        CmdSetDefaultiStatus();
                        UpdateMovement();
                        UseElectricWeapons();
                        break;
                    }
                case Status.giddy:
                    {
                        Giddy(2.0f);
                        break;
                    }
                case Status.opposite:
                    {
                        opposite_forward = -1;
                        opposite_rotate = 1;
                        UpdateMovement();
                        UseElectricWeapons();
                        Opposite(8.0f);
                        break;
                    }
                case Status.slow:
                    {
                        opposite_forward = 0.5f;
                        UpdateMovement();
                        UseElectricWeapons();
                        SlowTime(10.0f);
                        break;
                    }
                case Status.help:
                    {
                        UpdateMovement();
                        HelpSlider(true);
                        break;
                    }
                case Status.dead:
                    {
                        CmdSetiStartus("dead");
                        UseMedicalBox();
                        break;
                    }
            }
        }
        else if(playerNumber == 2)
        {
            SetModel(0, 2, 3);

            SmallSkillCooling(boss1_small_skill_time);
            BigSkillCooling(boss1_big_skill_time);
            SetSmallCoolingTime(tMedicalNums, boss1_small_skill_time);
            SetBigCoolingTime(tElectricWeaponNums, boss1_big_skill_time);
            switch (status)
            {
                case Status.normal:
                    {
                        CmdSetDefaultiStatus();
                        Boss1SmallSkill();
                        Boss1BigSkill();
                        UpdateMovement();
                        break;
                    }
                case Status.giddy:
                    {
                        Giddy(2.0f);
                        break;
                    }
            }
        }
        else if (playerNumber == 3)
        {
            SetModel(0, 1, 3);

            SmallSkillCooling(boss2_small_skill_time);
            BigSkillCooling(boss2_big_skill_time);
            SetSmallCoolingTime(tMedicalNums, boss2_small_skill_time);
            SetBigCoolingTime(tElectricWeaponNums, boss2_big_skill_time);
            switch (status)
            {
                case Status.normal:
                    {
                        CmdSetDefaultiStatus();
                        Boss2SmallSkill();
                        Boss2BigSkill();
                        UpdateMovement();
                        break;
                    }
                case Status.giddy:
                    {
                        Giddy(2.0f);
                        break;
                    }
            }
        }
        else if (playerNumber == 4)
        {
            SetModel(0, 1, 2);

            SmallSkillCooling(boss3_small_skill_time);
            BigSkillCooling(boss3_big_skill_time);
            SetSmallCoolingTime(tMedicalNums, boss3_small_skill_time);
            SetBigCoolingTime(tElectricWeaponNums, boss3_big_skill_time);
            switch (status)
            {
                case Status.normal:
                    {
                        CmdSetDefaultiStatus();
                        Boss3SmallSkill();
                        Boss3BigSkill();
                        UpdateMovement();
                        break;
                    }
                case Status.giddy:
                    {
                        Giddy(2.0f);
                        break;
                    }
            }
        }
    }

    private void UpdateMovement() {

        CmdSynWalk(false);
        walkFlag = false;
        if(playerNumber == 1)
        {
            animator.SetBool("Walking", walkFlag);
        }

        if (!canMove) { //Return if player can't move
            return;
        }
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement() {

        if (Input.GetKey(KeyCode.W))
        { //Up movement
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed) * opposite_forward;
            myTransform.rotation = Quaternion.Euler(0, 180*opposite_rotate + 0, 0);
            walkFlag = true;
            if (playerNumber == 1)
            {
                animator.SetBool("Walking", walkFlag);
            }
            CmdSynWalk(true);
        }

        if (Input.GetKey(KeyCode.A))
        { //Left movement
            rigidBody.velocity = new Vector3(-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z) * opposite_forward;
            myTransform.rotation = Quaternion.Euler(0, 180 * opposite_rotate + 270, 0);
            walkFlag = true;
            if (playerNumber == 1)
            {
                animator.SetBool("Walking", walkFlag);
            }
            CmdSynWalk(true);

        }

        if (Input.GetKey(KeyCode.S))
        { //Down movement
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed) * opposite_forward;
            myTransform.rotation = Quaternion.Euler(0, 180 * opposite_rotate + 180, 0);
            walkFlag = true;
            if (playerNumber == 1)
            {
                animator.SetBool("Walking", walkFlag);
            }
            CmdSynWalk(true);

        }

        if (Input.GetKey(KeyCode.D))
        { //Right movement
            rigidBody.velocity = new Vector3(moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z) * opposite_forward;
            myTransform.rotation = Quaternion.Euler(0, 180 * opposite_rotate + 90, 0);
            walkFlag = true;
            if (playerNumber == 1)
            {
                animator.SetBool("Walking", walkFlag);
            }
            CmdSynWalk(true);

        }

        if (canDropBombs && Input.GetKeyDown(KeyCode.Space))
        { //Drop bomb
            if (bombPrefab && BombNums > 0)
            { //可以释放炸弹
                BombNums--;
                CmdDropBomb();       
            }
                 
        }      
    }
    /// <summary>
    /// 设置模型
    /// </summary>
    /// <param name="n1">禁用模型n1</param>
    /// <param name="n2">禁用模型n2</param>
    /// <param name="n3">禁用模型n3</param>
    private void SetModel(int n1,int n2,int n3)
    {
        this.transform.Find("PlayerModel").GetChild(n1).gameObject.SetActive(false);
        this.transform.Find("PlayerModel").GetChild(n2).gameObject.SetActive(false);
        this.transform.Find("PlayerModel").GetChild(n3).gameObject.SetActive(false);
    }

    /// <summary>
    /// 使用物品
    /// </summary>
    private void UseElectricWeapons()
    {
        
        if(EleWeaponNums > 0)
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                //使用电击武器
                EleWeaponNums--;
                CmdUseWeapons();
            }
        }
    }
    [Command]
    void CmdUseWeapons()
    {
        var btmp = Instantiate(eleWeaponPrefab,
                    new Vector3(Mathf.RoundToInt(myTransform.position.x), eleWeaponPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z)),
                    eleWeaponPrefab.transform.rotation);
        NetworkServer.Spawn(btmp);
    }

    private void UseMedicalBox()
    {
        if ((MedicalNums > 0)&&(Input.GetKey(KeyCode.J)))
        {
            CmdSliderSetActive(true);
            if (this.GetComponent<SurvivalSlider>().processOver)
            {
                //使用医疗包
                MedicalNums--;
                status = Status.normal;
                this.GetComponent<SurvivalSlider>().processOver = false;
                this.GetComponent<SurvivalSlider>().processValue = 0.0f;
                this.GetComponent<SurvivalSlider>().slider.value = 0.0f;
                CmdSliderSetActive(false);
            }
        }
        else
        {
            CmdSliderSetActive(false);
        }
    }
    [Command]
    void CmdSliderSetActive(bool value)
    {
        RpcSliderSetActive(value);
    }
    [ClientRpc]
    void RpcSliderSetActive(bool value)
    {
        slider.SetActive(value);
    }

    [Command]
    private void CmdSynWalk(bool value)
    {
        animator.SetBool("Walking", value);
    }

    private void walkStatusChange(bool value)
    {
        animator.SetBool("Walking", value);
    }

    /// <summary>
    /// DropBomb 设置炸弹,BombNums减一;
    /// </summary>
    [Command]
    void CmdDropBomb() {
        var btmp = Instantiate(bombPrefab,
                    new Vector3(Mathf.RoundToInt(myTransform.position.x), bombPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z)),
                    bombPrefab.transform.rotation);
        NetworkServer.Spawn(btmp);
    }

    /// <summary>
    /// 眩晕时长
    /// </summary>
    /// <param name="totalTime">眩晕总时间</param>
    public void Giddy(float totalTime)
    {
        if(giddyTime < totalTime)
        {
            //iStatus.color = new Color(255, 255, 255, 255);
            //iStatus.sprite = Resources.Load<Sprite>("giddy");

            CmdSetiStartus("giddy");
            giddyTime += Time.deltaTime;
        }
        else
        {
            status = Status.normal;
            giddyTime = 0.0f;
        }
    }
    /// <summary>
    /// 持续相反状态
    /// </summary>
    /// <param name="totalTime"></param>
    private void Opposite(float totalTime)
    {
        if (oppositeTime < totalTime)
        {
            //iStatus.color = new Color(255, 255, 255, 255);
            //iStatus.sprite = Resources.Load<Sprite>("opposite");
            CmdSetiStartus("opposite");
            oppositeTime += Time.deltaTime;
        }
        else
        {
            status = Status.normal;
            oppositeTime = 0.0f;
        }
    }
    /// <summary>
    /// 减速状态
    /// </summary>
    /// <param name="totalTime"></param>
    private void SlowTime(float totalTime)
    {
        if (slowTime < totalTime)
        {
            CmdSetiStartus("slow");
            slowTime += Time.deltaTime;
        }
        else
        {
            status = Status.normal;
            slowTime = 0.0f;
        }
    }
    [Command]
    void CmdSetDefaultiStatus()
    {
        iStatus.color = new Color(255, 255, 255, 0);
        slider.SetActive(false);
        CmdSliderSetActive(false);
        RpcSetDefaultiStatus();
    }
    [ClientRpc]
    void RpcSetDefaultiStatus()
    {
        iStatus.color = new Color(255, 255, 255, 0);
        slider.SetActive(false);
    }

    [Command]
    void CmdSetiStartus(string var)
    {
        iStatus.color = new Color(255, 255, 255, 255);
        iStatus.sprite = Resources.Load<Sprite>(var);
        RpcSetiStatus(var);
    }
    [ClientRpc]
    void RpcSetiStatus(string var)
    {
        iStatus.color = new Color(255, 255, 255, 255);
        iStatus.sprite = Resources.Load<Sprite>(var);
    }


    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (!dead && other.CompareTag("Explosion"))
        { //Not dead & hit by explosion
            Debug.Log("P" + playerNumber + " hit by explosion!");
            if(this.playerNumber == 1)
            {
                status = Status.dead;
            }
            else
            {
                status = Status.giddy;
            }
        }     
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(!isLocalPlayer)
        {
            return;
        }
        if (status != Status.dead && collision.gameObject.CompareTag("Player"))
        {
            if ((collision.gameObject.GetComponent<Player>().playerNumber == 1)
                &&(collision.gameObject.GetComponent<Player>().status == Status.dead))
            {
                CmdHelpedFriend(collision.gameObject);
                status = Status.help;
            }
        }
    }
    [Command]
    void CmdHelpedFriend(GameObject tmp)
    {
        helpedFriend = tmp;
        RpcHelpedFriend(helpedFriend);
    }
    [ClientRpc]
    void RpcHelpedFriend(GameObject tmp)
    {
        helpedFriend = tmp;
    }

    public void OnCollisionExit(Collision collision)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (status != Status.dead && collision.gameObject.CompareTag("Player"))
        {
            status = Status.normal;
            this.GetComponent<SurvivalSlider>().processOver = false;
            this.GetComponent<SurvivalSlider>().processValue = 0.0f;
            this.GetComponent<SurvivalSlider>().slider.value = 0.0f;
        }
    }

    public void HelpSlider(bool value)
    {
        CmdSliderSetActive(value);
        if (this.GetComponent<SurvivalSlider>().processOver)
        {
            CmdHelpFriend();
            this.GetComponent<SurvivalSlider>().processOver = false;
            this.GetComponent<SurvivalSlider>().processValue = 0.0f;
            this.GetComponent<SurvivalSlider>().slider.value = 0.0f;
            status = Status.normal;
        }
    }
    [Command]
    public void CmdHelpFriend()
    {
        RpcHelpFriend();
    }
    [ClientRpc]
    public void RpcHelpFriend()
    {
        helpedFriend.GetComponent<Player>().status = Status.normal;
    }

    public void SmallSkillCooling(float totalTime)
    {
        if (smallSkillTime < totalTime)
        {
            smallSkillTime += Time.deltaTime;
        }
        else
        {
            smallSkillTime = 0.0f;
            flagSmall = true;
        }
    }
    public void BigSkillCooling(float totalTime)
    {
        if (bigSkillTime < totalTime)
        {
            bigSkillTime += Time.deltaTime;
        }
        else
        {
            bigSkillTime = 0.0f;
            flagBig = true;
        }
    }

    #region Boss1 Skills
    private void Boss1SmallSkill()
    {
        //放置障碍物（冷却时间：2秒）
        if(flagSmall&&Input.GetKey(KeyCode.J))
        {
            CmdPlaceBox();
            flagSmall = false;
        }
    }
    private void Boss1BigSkill()
    {
        //刷新全图box
        if(flagBig&&Input.GetKey(KeyCode.K))
        {
            CmdFreshAllBox();
            flagBig = false;
        }

    }
    [Command]
    void CmdFreshAllBox()
    {
        GameObject.Find("Global State Manager").GetComponent<GlobalStateManager>().CmdBoxProduct(20);
    }

    [Command]
    void CmdPlaceBox()
    {
        var btmp = Instantiate(boxPrefab,
                    new Vector3(Mathf.RoundToInt(myTransform.position.x), boxPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z)),
                    boxPrefab.transform.rotation);
        NetworkServer.Spawn(btmp);
    }
    #endregion

    #region Boss2 Skills
    private void Boss2SmallSkill()
    {
        //朝前扔出一个飞镖立马引爆炸弹（冷却时间5秒）
        if (flagSmall && Input.GetKey(KeyCode.J))
        {
            CmdThrowDarts();
            flagSmall = false;
        }

    }
    private void Boss2BigSkill()
    {
        //朝身体四个方向同时射出飞镖（冷却时间30秒）
        if (flagBig && Input.GetKey(KeyCode.K))
        {
            CmdThrowAllDarts();
            flagBig = false;
        }
    }

    [Command]
    void CmdThrowDarts()
    {
        var btmp = Instantiate(dartsPrefab,
                    Hand[0].position,
                    Hand[0].rotation);
        NetworkServer.Spawn(btmp);
    }
    [Command]
    void CmdThrowAllDarts()
    {
        var btmp1 = Instantiate(dartsPrefab,
                    Hand[0].position,
                    Hand[0].rotation);
        var btmp2 = Instantiate(dartsPrefab,
                    Hand[1].position,
                    Hand[1].rotation);
        var btmp3 = Instantiate(dartsPrefab,
                    Hand[2].position,
                    Hand[2].rotation);
        var btmp4 = Instantiate(dartsPrefab,
                    Hand[3].position,
                    Hand[3].rotation);
        NetworkServer.Spawn(btmp1);
        NetworkServer.Spawn(btmp2);
        NetworkServer.Spawn(btmp3);
        NetworkServer.Spawn(btmp4);

    }
    #endregion

    #region Boss3 Skills
    private void Boss3SmallSkill()
    {
        //放置香蕉皮，玩家踩中后速度降低为原来的一半，持续10秒，冷却时间5秒
        if (flagSmall && Input.GetKey(KeyCode.J))
        {
            CmdBanana();
            flagSmall = false;
        }

    }
    private void Boss3BigSkill()
    {
        //两极反转，游戏中所有玩家控制反向，持续5秒，冷却时间30秒
        if (flagBig && Input.GetKey(KeyCode.K))
        {
            CmdOpposite();
            flagBig = false;
        }
    }
    [Command]
    void CmdBanana()
    {
        var btmp = Instantiate(bananaPrefab,
                    new Vector3(Mathf.RoundToInt(myTransform.position.x), bananaPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z)),
                    bananaPrefab.transform.rotation);
        NetworkServer.Spawn(btmp);
    }

    [Command]
    void CmdOpposite()
    {
        RpcOpposite();
    }
    [ClientRpc]
    void RpcOpposite()
    {
        foreach(var tmp in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (tmp.GetComponent<Player>().playerNumber == 1)
            {
                tmp.GetComponent<Player>().status = Status.opposite;
            }
        }
        
    }
    #endregion

    #region 设置技能图标
    private void SetSkillIcons(string iconName,Image tmp)
    {
        tmp.sprite = Resources.Load<Sprite>(iconName);
    }
    private void SetSmallCoolingTime(Text tmp,float coolingTime)
    {
        if(!flagSmall)
        {
            tmp.text = "      冷却时间: "+(coolingTime - (int)smallSkillTime).ToString();
        }
    }
    private void SetBigCoolingTime(Text tmp,float coolingTime)
    {
        if(!flagBig)
        {
            tmp.text = "      冷却时间: " + (coolingTime - (int)bigSkillTime).ToString();
        }
    }
    #endregion
}
