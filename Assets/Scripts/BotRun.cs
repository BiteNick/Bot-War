using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;


public class BotRun : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 point;
    private Animator anim;
    public State currentState;
    [SerializeField] public State MoveState;
    [SerializeField] public State AttackState;
    [SerializeField] public State DefendState;
    [SerializeField] public State ReloadState;
    [SerializeField] public State shotGunDeployState;
    [SerializeField] public State GrenadeThrowState;
    [SerializeField] public Gun Gun;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxhp = 50f;
    [SerializeField] private float hp;
    [SerializeField] private float hpIncrement = 10f; //using this variable when upgrading hp
    [SerializeField] private int kills = 0;

    [SerializeField] private int spreadDefuffDefault = 0;
    [SerializeField] public float spreadDebuff = 0; //увеличение точности (или уменьшение, если отрицательно)
    private int spreadDebuffSit = 2;

    [SerializeField] private float distance; //radius start attacks
    [SerializeField] private int grenadeThrowChancePercents = 12;
    [SerializeField] public GameObject currentPositionGameObject; //текущее укрытие объекта
    [SerializeField] private string EnemiesTag; //tag of enemies this object
    [SerializeField] private string looseTriggerTag;
    [SerializeField] private Collider[] enemiesCollisions;
    [SerializeField] public GameObject target;
    private float timeToKillTargetMax = 1.5f;
    private float timeToKillTarget;
    [SerializeField] private float timeToThrowGrenadeMax = 15f;
    private float timeToThrowGrenade = 0f;
    [SerializeField] private BotRun targetScript;
    [SerializeField] public Transform attackPosition; //маркер для стрельбы по врагам
    [SerializeField] float AngleInDegrees = 170f;
    private float markerOffsetYSit = 1.6f; //отклонение маркера по высоте, если цель сидит
    private float markerOffsetYStay = 2.3f; //отклонение маркера по высоте, если цель стоит
    [SerializeField] public float delayShot; //время ожидания между выстрелами
    [SerializeField] public bool StayAtPosition; //будет ли стоять на одной позиции всю игру
    public bool MovedToPosition = false; //дошёл ли до позиции
    private MultiAimConstraint AimConstraint;
    private RigBuilder rigBuilder;
    private WaitForSeconds delayDetector;
    private BoxCollider _collider;
    private Vector3 standartSizeCollider = new Vector3(1f, 2.5f, 1f);
    private Vector3 standartCenterCollider = new Vector3(0, 1.25f, 0);
    [SerializeField] private Collider[] ragdollColliders;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    private Transform leftHand;
    [SerializeField] private GameObject Hat;
    public GameObject grenadeObject;
    public GameObject currentGrenade;
    private float g = Physics.gravity.y;
    [SerializeField] private Transform grenadeMarker; //where is throwing grenades
    [HideInInspector] public bool skipPosition = false; // bool for MoveState

    private SkinnedMeshRenderer meshRenderer;
    private Material[] objMaterials;
    [SerializeField] private Material materialBlink;

    [SerializeField] private GameObject healthUpAnim;
    [SerializeField] private GameObject healthUpParticle;

    [SerializeField] private GameObject blindSprite;
    bool isBlinded = false;

    private void Awake()
    {
        materialBlink = Resources.Load("MaterialBlink", typeof(Material)) as Material;
        meshRenderer = transform.Find("obj").GetComponent<SkinnedMeshRenderer>();
        objMaterials = meshRenderer.sharedMaterials;

        leftHand = transform.Find("metarig/spine/spine.001/spine.002/spine.003/shoulder.L/upper_arm.L/forearm.L/hand.L/hand.L_end");
        if (gameObject.CompareTag("bot_enemy"))
        {
            EnemiesTag = "bot_ally";
            looseTriggerTag = "looseTriggerAlly";

        }
        else if (gameObject.CompareTag("bot_ally"))
        {
            EnemiesTag = "bot_enemy";
            looseTriggerTag = "looseTriggerEnemy";
        }
        _collider = GetComponent<BoxCollider>();
    }


    void Start()
    {
        Gun.setCharacter(this);
        if (currentPositionGameObject != null)
        {
            gameManagerStatic.SetPosition(currentPositionGameObject, true, gameObject);
        }
        timeToKillTarget = timeToKillTargetMax;
        hp = maxhp;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        anim = gameObject.GetComponent<Animator>();
        delayDetector = new WaitForSeconds(0.3f);
        delayShot = Gun.FireRate;

        AimConstraint = transform.Find("Rig1/constraint").GetComponent<MultiAimConstraint>();
        rigBuilder = GetComponent<RigBuilder>();
        SetTarget(attackPosition);

        
        if (StayAtPosition)
        {
            SetState(AttackState);
            spreadDebuff = spreadDebuffSit;
        }
        else
        {
            SetState(MoveState);
        }
        StartCoroutine("DetectEnemies");
        StartCoroutine("attackPositionChange");

        RagdollSwitch(false);
        distance = Gun.fireDistance;
    }
    void Update()
    {
        currentState.Run();
        timeToKillTarget -= Time.deltaTime;
        timeToThrowGrenade -= Time.deltaTime;
    }

    public void SetState(State state)
    {
        SetColliderSize(standartSizeCollider, standartCenterCollider);
        State newState = Instantiate(state);
        spreadDebuff = spreadDefuffDefault;
        StopAgent(true);
        newState.preInit();
        currentState = newState;
        currentState.character = this;
        currentState.Init();
    }


    public bool TakeDamage(float damage) //returns true if this object killed
    {
        hp -= damage;
        Invoke("blinkMaterial", 0.2f);

        if (hp <= 0)
        {
            if (currentPositionGameObject != null && MovedToPosition)
            {
                gameManagerStatic.SetPosition(currentPositionGameObject, false, gameObject);
                gameManagerStatic.turnOffButtons(currentPositionGameObject);
            }

            RagdollSwitch(true);

            if (Hat != null)
            {
                Hat.GetComponent<BoxCollider>().enabled = true;
                Hat.GetComponent<Rigidbody>().isKinematic = false;
            }

            Destroy(gameObject, 5f);
            return true;
        }
        if (!StayAtPosition && currentState.StateName != "SitDefendingState" && currentState.StateName != "ReloadState" && currentState.StateName != "grenadeThrowState" && (agent.destination - transform.position).magnitude > 10f)
        {
            CheckStayState();
        }


        return false;
    }

    private void blinkMaterial()
    {
        meshRenderer.sharedMaterials = new Material[] { materialBlink, materialBlink };
        Invoke("returnMaterials", 0.15f);
    }

    private void returnMaterials()
    {
        meshRenderer.sharedMaterials = objMaterials;
    }

    public void IncreaseHp() //it is doing if object dead
    {
        if (hp > 0)
        {
            if (healthUpAnim != null)
            {
                createEffect(healthUpAnim, new Vector3(0f, 4f, 0f), Quaternion.identity, true, true);
            }
            if (healthUpParticle != null)
            {
                createEffect(healthUpParticle, new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0), false, false);
            }

            maxhp += hpIncrement;
            hp += hpIncrement;
        }
    }


    public void CheckStayState() //check how position to need (stay or sit)
    {
        if (grenadeObject != null && target != null && Random.Range(1, 100) <= grenadeThrowChancePercents && timeToThrowGrenade < 0f)
        {
            Invoke("GrenadeThrowChangeState", 0.1f); //changeState to grenadeThrow
            timeToThrowGrenade = timeToThrowGrenadeMax;
        }
        if (currentState.StateName != "ShootState" && maxhp / 2 <= hp && !MovedToPosition)
        {
            SetState(AttackState); //changeState
        }
        else if (currentState.StateName != "SitDefendingState" && maxhp / 2 > hp || MovedToPosition)
        {
            SetState(DefendState); //changeState
        }
    }

    private void GrenadeThrowChangeState()
    {
        SetState(GrenadeThrowState);
    }

    public void SetColliderSize(Vector3 size, Vector3 center)
    {
        _collider.size = size;
        _collider.center = center;
    }


    public void animSetBool(string animBool)
    {
        animClearBools();
        anim.SetBool(animBool, true);
    }


    public void animClearBools()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                anim.SetBool(parameter.name, false);
        }
    }


    public void animSetLegsState(int numberState) //0 - Run, 1 - Stay, 2 - Sit
    {
        anim.SetInteger("LegsState", numberState);
    }


    public void animSetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void setMovePosition(Vector3 movePoint)
    {
        StopAgent(false);
        point = movePoint;
        agent.SetDestination(point);
    }

    public void kill()
    {
        kills++;
    }

    public void SetTarget(Transform attackPos)
    {
        var data = AimConstraint.data.sourceObjects;
        data.Insert(0, new WeightedTransform { weight = 1 });
        data.SetTransform(0, attackPos.transform);
        if (data.Count > 1)
        {
            data.RemoveAt(1);
        }
        AimConstraint.data.sourceObjects = data;
        rigBuilder.Build();
    }

    public void EnableRigBuilder(bool enableRig) //if true, rig is going to enable
    {
        AimConstraint.enabled = enableRig;
        rigBuilder.enabled = enableRig;
    }


    public void ClearTarget() //нужен в случаях, если будут состояния по типу бега
    {
        var data = AimConstraint.data.sourceObjects;
        data.Clear();
        AimConstraint.data.sourceObjects = data;
        rigBuilder.Build();
    }


    IEnumerator DetectEnemies()
    {
        while (true)
        {
            if (target == null || targetScript.enabled == false || timeToKillTarget <= 0f )
            {
                target = null; //чтобы не пинали убитого противника
                enemiesCollisions = Physics.OverlapSphere(transform.position, distance, 1<<8);
                foreach (var item in enemiesCollisions)
                {
                    if (item.CompareTag(EnemiesTag))
                    {
                        timeToKillTarget = timeToKillTargetMax;
                        target = item.gameObject;
                        targetScript = target.GetComponent<BotRun>();
                        RaycastHit hit;
                        Vector3 markerItemPosition;
                        if (targetScript != null && targetScript.currentState != null && targetScript.currentState.StateName == "SitDefendingState")
                        {
                            markerItemPosition = item.transform.position + new Vector3(0f, markerOffsetYSit, 0f);
                        }
                        else
                        {
                            markerItemPosition = item.transform.position + new Vector3(0f, markerOffsetYStay, 0f);
                        }
                        Vector3 heading = markerItemPosition - Gun.SpawnBulletPos.position;
                        Ray ray = new Ray(Gun.SpawnBulletPos.position, heading / heading.magnitude);
                        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(EnemiesTag))
                        {

                            if (currentState.StateName == "MoveState" && (agent.destination - transform.position).magnitude > 15f)
                            {
                                CheckStayState(); //ChangeState
                            }
                            break;
                        }
                    }
                }
            }
            attackPositionChange();
            yield return delayDetector;
        }
    }


    private void attackPositionChange()
    {
        if (target != null)
        {
            attackPosition.position = target.transform.position;
            if (targetScript.currentState != null && targetScript.currentState.StateName == "SitDefendingState")
            {
                attackPosition.position += new Vector3(0f, markerOffsetYSit, 0f);
            }
            else
            {
                attackPosition.position += new Vector3(0f, markerOffsetYStay, 0f);
            }
        }
    }


    private void RagdollSwitch(bool state) //если true, то объект становится динамичным (включается ragdoll)
    {
        anim.enabled = !state; //аниматор вырубается, если подаётся true
        foreach (Collider collider in ragdollColliders)
        {
            collider.enabled = state;
        }
        foreach (Rigidbody rbItem in ragdollRigidbodies)
        {
            rbItem.isKinematic = !state; //потому что если isKinematic true, то он делает физику объекта статичной
            rbItem.detectCollisions = state;
            rbItem.useGravity = state;
        }

        Gun.GetComponent<BoxCollider>().enabled = state;
        _collider.enabled = !state; //выключение основного кубического коллайдера, если true
        agent.enabled = !state; //выключение NavMeshAgent, если true
        rigBuilder.enabled = !state; //выключение RigBuilder, если true
        this.enabled = !state; //выключение данного скрипта, если true
    }


    public void ReloadStart() //Начало перезардяки
    {
        SetState(ReloadState);
        Invoke("refillMagazine", 2.85f);
    }


    public void MagazineDetach()
    {
        Gun.MagazineDetach();
    }

    public void MagazinePickUp()
    {
        if (Gun.MagazineObject != null)
        {
            GameObject magazine = Instantiate(Gun.MagazineObject, leftHand, leftHand);
            magazine.transform.localPosition = new Vector3(-0.00011f, -0.00015f, 0.00052f);
            magazine.transform.rotation = Quaternion.Euler(0, 180f, 0);
            Gun.MagazinePickUp(magazine);
        }
    }

    public void MagazineAttach()
    {
        if (Gun.MagazineObject != null)
        {
            Gun.MagazineAttach();
        }
    }

    public void refillMagazine() //Конец перезарядки
    {
        Gun.refillMagazine();
        CheckStayState();
    }


    public void GrenadePickUp()
    {
        currentGrenade = Instantiate(grenadeObject, leftHand);
        currentGrenade.GetComponent<grenade>().GrenadeInit(this, EnemiesTag);
        currentGrenade.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        currentGrenade.transform.localPosition = new Vector3(0, 0, 0);
        

    }


    public void ThrowGrenade()
    {
        currentGrenade.transform.parent = null;
        currentGrenade.GetComponent<CapsuleCollider>().enabled = true;
        currentGrenade.GetComponent<Rigidbody>().isKinematic = false;

        Vector3 throwTo = new Vector3(attackPosition.position.x, attackPosition.position.y-1f, attackPosition.position.z);
        Vector3 fromTo = throwTo - grenadeMarker.position;
        Vector3 fromToXZ = new Vector3(fromTo.x, 0f, fromTo.z);


        grenadeMarker.rotation = Quaternion.LookRotation(fromToXZ, Vector3.up);
        grenadeMarker.localEulerAngles = new Vector3(-AngleInDegrees, grenadeMarker.localEulerAngles.y, grenadeMarker.localEulerAngles.z);


        float x = fromToXZ.magnitude;
        float y = fromTo.y;

        float AngleInRadians = AngleInDegrees * Mathf.PI / 180;

        float v2 = (g * x * x) / (2 * (y - Mathf.Tan(AngleInRadians) * x) * Mathf.Pow(Mathf.Cos(AngleInRadians), 2));
        float v = Mathf.Sqrt(Mathf.Abs(v2));
        currentGrenade.GetComponent<Rigidbody>().velocity = grenadeMarker.forward * v;

    }

    public void shotGunUnwrapping()
    {
        Gun.animSetTrigger("unwrapTrigger");
    }

    public void StopAgent(bool AgentState) // if true, agent is stopping
    {
        if (AgentState)
            agent.speed = 0f;
        else
            agent.speed = speed;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag(looseTriggerTag))
        {
            StartGameScript startGameScriptTemp = GameObject.FindGameObjectWithTag("GameManager").GetComponent<StartGameScript>();
            startGameScriptTemp.GameEnd(gameObject.tag);
        }
    }

    public void addSpreadDebuff(float spreadBuff, float timeStep)
    {
        spreadDebuff = -spreadBuff;
        Invoke("defaultSpread", timeStep);
        Destroy(createEffect(blindSprite, new Vector3(0f, 4f, 0f), Quaternion.identity, true, false), timeStep);

    }

    public void defaultSpread()
    {
        spreadDebuff = spreadDefuffDefault;
    }

    private GameObject createEffect(GameObject original, Vector3 positionOffset, Quaternion rotationOffset, bool isSprite, bool isAnim)
    {
        GameObject effectGameObject = Instantiate(original, transform.position, rotationOffset);
        effectGameObject.GetComponent<EffectsMove>().myConstructor(positionOffset, this, isSprite, isAnim);
        return effectGameObject;
    }

}
