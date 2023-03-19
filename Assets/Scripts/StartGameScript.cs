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
    private GameObject currentSpawnUnit;
    private float currentSpawnUnitCost;

    [SerializeField]private levelWaves levelWaves;

    [SerializeField] private float resources = 1f;
    private float income_resources = 1f;
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

}
