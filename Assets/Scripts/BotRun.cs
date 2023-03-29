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
    [SerializeField] public Gun Gun;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxhp = 50f;
    [SerializeField] private float hp;
    [SerializeField] private int kills = 0;
    [SerializeField] public int spreadDebuff = 0; //увеличение точности
    [SerializeField] private float distance = 40f; //радиус обнаружения врагов
    [SerializeField] public GameObject currentPositionGameObject; //текущее укрытие объекта
    [SerializeField] private string EnemiesTag; //враги относительно этого объекта
    [SerializeField] private Collider[] enemiesCollisions;
    [SerializeField] public GameObject target;
    private float timeToKillTargetMax = 5f;
    private float timeToKillTarget;
    [SerializeField] private BotRun targetScript;
    [SerializeField] public Transform attackPosition; //маркер для стрельбы по врагам
    private float markerOffsetYSit = 1.6f; //отклонение маркера по высоте, если цель сидит
    private float markerOffsetYStay = 2.3f; //отклонение маркера по высоте, если цель стоит
    [SerializeField] public float delayShot; //время ожидания между выстрелами
    [SerializeField] public bool StayAtPosition; //будет ли стоять на одной позиции всю игру
    public bool MovedToPosition = false; //дошёл ли до позиции
    private MultiAimConstraint AimConstraint;
    private RigBuilder rigBuilder;
    private WaitForSeconds delayDetector;
    private WaitForSeconds delayPositionChange;
    private BoxCollider _collider;
    private Vector3 standartSizeCollider = new Vector3(1f, 2.5f, 1f);
    private Vector3 standartCenterCollider = new Vector3(0, 1.25f, 0);

    void Start()
    {
        Gun.setCharacter(this);
        _collider = GetComponent<BoxCollider>();
        if (currentPositionGameObject != null)
        {
            gameManagerStatic.SetPosition(currentPositionGameObject, true, gameObject);
        }
        timeToKillTarget = timeToKillTargetMax;
        hp = maxhp;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        anim = gameObject.GetComponent<Animator>();
        delayDetector = new WaitForSeconds(0.5f);
        delayPositionChange = new WaitForSeconds(0.1f);
        delayShot = Gun.FireRate;

        AimConstraint = transform.Find("Rig1/constraint").GetComponent<MultiAimConstraint>();
        rigBuilder = GetComponent<RigBuilder>();
        SetTarget(attackPosition);

        if (gameObject.CompareTag("bot_enemy"))
        {
            EnemiesTag = "bot_ally";
        }
        else if (gameObject.CompareTag("bot_ally"))
        {
            EnemiesTag = "bot_enemy";
        }
        if (StayAtPosition)
        {
            SetState(AttackState);
            spreadDebuff = 2;
        }
        else
        {
            SetState(MoveState);
        }
        StartCoroutine("DetectEnemies");
        StartCoroutine("attackPositionChange");
        

    }

    void Update()
    {
        currentState.Run();
        timeToKillTarget -= Time.deltaTime;
    }

    public void SetState(State state)
    {
        SetColliderSize(standartSizeCollider, standartCenterCollider);
        State newState = Instantiate(state);
        newState.preInit();
        spreadDebuff = 0;
        if (newState.StateName == "MoveState" && currentPositionGameObject != null)
        {
            gameManagerStatic.SetPosition(currentPositionGameObject, false, gameObject); //обнуление позиции перса при изменении состояния на бег
            currentPositionGameObject = null;
        }
        
        agent.isStopped = true;
        SetTarget(attackPosition);
        currentState = newState;
        currentState.character = this;
        currentState.Init();

        if (currentPositionGameObject != null)
        {
            gameManagerStatic.SetPosition(currentPositionGameObject, true, gameObject);
        }
    }


    public bool TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            if (currentPositionGameObject != null)
            {
                gameManagerStatic.SetPosition(currentPositionGameObject, false, gameObject);
                if (currentPositionGameObject != null)
                {
                    gameManagerStatic.turnOffButtons(currentPositionGameObject);
                }
            }
            Destroy(gameObject);
            return true;
        }
        if (!StayAtPosition && currentState.StateName != "SitDefendingState" && (agent.destination - transform.position).magnitude > 10f)
        {
            if (currentState.StateName != "ShootState" && maxhp / 2 <= hp)
            {
                SetState(AttackState); //changeState
            }
            else if (currentState.StateName != "SitDefendingState" && maxhp / 2 > hp)
            {
                SetState(DefendState); //changeState
            }
        }
        return false;
    }


    public void SetColliderSize(Vector3 size, Vector3 center)
    {
        _collider.size = size;
        _collider.center = center;
    }


    public void animSetBool(string animBool)
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
        anim.SetBool(animBool, true);
    }

    public void setMovePosition(Vector3 movePoint)
    {
        agent.isStopped = false;
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
            if (target == null || timeToKillTarget < 0f)
            {
                timeToKillTarget = timeToKillTargetMax;
                enemiesCollisions = Physics.OverlapSphere(transform.position, distance);
                foreach (var item in enemiesCollisions)
                {
                    if (item.CompareTag(EnemiesTag))
                    {
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
                                TakeDamage(0); //ChangeState (takeDamage для перенаправления в правильную позицию)
                            }
                            break;
                        }
                    }
                }
            }
            yield return delayDetector;
        }
    }
    

    IEnumerator attackPositionChange()
    {
        while (true)
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
            yield return delayPositionChange;
        }
    }


}
