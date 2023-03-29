using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameScript : MonoBehaviour //responsible for resources and spawn user's units
{
    [SerializeField] private GameObject AkGuy;
    private float AkGuyCost = 3f;
    [SerializeField] private GameObject MachineGunGuy;
    private float MachineGunGuyCost = 5f;
    [SerializeField] private GameObject SniperGuy;
    private float SniperGuyCost = 6f;
    private GameObject currentSpawnUnit;
    private float currentSpawnUnitCost;

    [SerializeField]private levelWaves levelWaves;

    [SerializeField] private float resources = 1f;
    [SerializeField] private float income_resources = 1f;
    private WaitForSeconds IncomeDelay = new WaitForSeconds(1f);
    [SerializeField] private Text ResourcesText;
    private void Awake()
    {
        gameManagerStatic.StartManager();
    }

    private void Start()
    {
        levelWaves = Instantiate(levelWaves);
        levelWaves.Init();
        StartCoroutine("Waves");
        StartCoroutine("Income");
    }

    public void SpawnUnit(string unitName)
    {
        if (unitName == "AkGuy")
        {
            currentSpawnUnit = AkGuy;
            currentSpawnUnitCost = AkGuyCost;
        }
        else if(unitName == "MachineGunGuy")
        {
            currentSpawnUnit = MachineGunGuy;
            currentSpawnUnitCost = MachineGunGuyCost;
        }


        else if(unitName == "SniperGuy")
        {
            currentSpawnUnit = SniperGuy;
            currentSpawnUnitCost = SniperGuyCost;
        }


        else
        {
            currentSpawnUnit = null;
        }
        if (currentSpawnUnit != null && resources >= currentSpawnUnitCost)
        {
            Instantiate(currentSpawnUnit, new Vector3(Random.Range(13f, levelWaves.mapWidthX-10), 0f, 3f), Quaternion.identity);
            resources -= currentSpawnUnitCost;
            ResourcesText.text = $"{resources}(+{income_resources})$";
        }
    }

    public void AttackButton(GameObject positionGroups) //units go attack when button attack pressed
    {
        foreach (Transform position in positionGroups.transform)
        {
            if (position.CompareTag("position"))
            {
                Collider[] unitsColliders = Physics.OverlapSphere(position.position, 3f);
                foreach (var item in unitsColliders)
                {
                    if (item.CompareTag("bot_ally"))
                    {
                        BotRun botScript = item.GetComponent<BotRun>();
                        if (botScript.currentState.StateName != "MoveState")
                            botScript.SetState(botScript.MoveState);
                    }
                }
                gameManagerStatic.turnButtons(position.gameObject);
            }

        }
    }


    

    private IEnumerator Income()
    {
        while (true)
        {
            resources += income_resources;
            ResourcesText.text = $"{resources}(+{income_resources})$";
            yield return IncomeDelay;
        }
    }

    private IEnumerator Waves()
    {
        while (true)
        {
            levelWaves.Run();
            yield return levelWaves.vavesDelay;
        }
    }

    public void Temp()
    {
        gameManagerStatic.Temp();
    }

}
