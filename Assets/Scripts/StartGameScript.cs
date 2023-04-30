using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour //responsible for resources and spawn player's units
{
    [SerializeField] private GameObject AkGuy;
    private float AkGuyCost = 3f;
    [SerializeField] private GameObject MachineGunGuy;
    private float MachineGunGuyCost = 5f;
    [SerializeField] private GameObject SniperGuy;
    private float SniperGuyCost = 6f;
    [SerializeField] private GameObject ShotGunnerGuy;
    private float ShotGunnerGuyCost = 10f;

    [SerializeField] private Text IncomeIncreaseButtonText;
    private int IncomeLevel = 0;
    private float[] IncomeLevelCost = new float[6] { 40, 60, 100, 150, 200, 250};

    [SerializeField] private Text hpIncreaseButtonText;
    private int hpLevel = 0;
    private float[] hpLevelCost = new float[16] {15, 25, 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 170, 200, 250, 300 };

    [SerializeField]private levelWaves levelWaves;

    [SerializeField] private float resources = 1f;
    [SerializeField] private float income_resources = 1f;
    private WaitForSeconds IncomeDelay = new WaitForSeconds(1f);
    [SerializeField] private Text ResourcesText;


    private GameObject TopPanel;
    private GameObject DefeatPanel;
    private GameObject WinPanel;
    private GameObject PausePanel;
    private GameObject currentPanel;

    private bool isPause = false;
    private bool isPausing = false; //true if doing pause

    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Slider CameraKeysMovingSpeedSlider;
    [SerializeField] private Slider CameraMoveSpeedSlider;

    private Animator PausePanelAnim;
    private Animator WinPanelAnim;
    private Animator DefeatPanelAnim;

    private void Awake()
    {
        gameManagerStatic.StartManager();
        TopPanel = GameObject.FindGameObjectWithTag("TopPanel");
        DefeatPanel = GameObject.FindGameObjectWithTag("DefeatPanel");
        DefeatPanel.SetActive(false);
        WinPanel = GameObject.FindGameObjectWithTag("WinPanel");
        WinPanel.SetActive(false);
        PausePanel = GameObject.FindGameObjectWithTag("PausePanel");
        PausePanel.SetActive(false);
    }

    private void Start()
    {
        PausePanelAnim = PausePanel.GetComponent<Animator>();
        WinPanelAnim = WinPanel.GetComponent<Animator>();
        DefeatPanelAnim = DefeatPanel.GetComponent<Animator>();

        if (VolumeSlider != null)
            VolumeSlider.value = gameManagerStatic.Volume;

        if (CameraKeysMovingSpeedSlider != null)
            CameraKeysMovingSpeedSlider.value = gameManagerStatic.CameraKeysMovingSpeed / gameManagerStatic.CameraMaxKeysMovingSpeed;
        if (CameraMoveSpeedSlider != null)
            CameraMoveSpeedSlider.value = gameManagerStatic.CameraMoveSpeed / gameManagerStatic.CameraMaxMoveSpeed;


        AudioListener.volume = gameManagerStatic.Volume; ;
        IncomeIncreaseButtonText.text = $"{IncomeLevelCost[0]}$";
        hpIncreaseButtonText.text = $"{hpLevelCost[0]}$";
        levelWaves = Instantiate(levelWaves);
        levelWaves.Init();
        StartCoroutine("Waves");
        StartCoroutine("Income");
    }

    public void spawnAkGuy()
    {
        SpawnUnit(AkGuy, AkGuyCost);
    }
    public void spawnSniper()
    {
        SpawnUnit(SniperGuy, SniperGuyCost);
    }
    public void spawnMachineGunGuy()
    {
        SpawnUnit(MachineGunGuy, MachineGunGuyCost);
    }
    public void spawnShotGunGuy()
    {
        SpawnUnit(ShotGunnerGuy, ShotGunnerGuyCost);
    }

    public void SpawnUnit(GameObject unit, float cost)
    {
        
        if (unit != null && resources >= cost)
        {
            GameObject spawnedUnit = Instantiate(unit, new Vector3(Random.Range(13f, levelWaves.mapWidthX - 10), 0f, 3f), Quaternion.identity);
            spawnedUnit.SetActive(true);
            resources -= cost;
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


    public void Income_Increase()
    {
        if (IncomeLevel < IncomeLevelCost.Length && resources >= IncomeLevelCost[IncomeLevel])
        {
            resources -= IncomeLevelCost[IncomeLevel];
            income_resources += 0.5f;
            IncomeLevel++;
            if (IncomeLevel + 1 > IncomeLevelCost.Length)
                IncomeIncreaseButtonText.text = "max lvl";
            else
                IncomeIncreaseButtonText.text = $"{IncomeLevelCost[IncomeLevel]}$";
            ResourcesText.text = $"{resources}(+{income_resources})$";
        }
    }


    public void Hp_Increase()
    {
        if (hpLevel < hpLevelCost.Length && resources >= hpLevelCost[hpLevel])
        {
            resources -= hpLevelCost[hpLevel];
            AkGuy.GetComponent<BotRun>().IncreaseHp();
            SniperGuy.GetComponent<BotRun>().IncreaseHp();
            MachineGunGuy.GetComponent<BotRun>().IncreaseHp();

            GameObject[] unitsOnBattleField = GameObject.FindGameObjectsWithTag("bot_ally");
            foreach(GameObject unit in unitsOnBattleField)
            {
                unit.GetComponent<BotRun>().IncreaseHp();
            }

            hpLevel++;
            if (hpLevel + 1 > hpLevelCost.Length)
                hpIncreaseButtonText.text = "max lvl";
            else
                hpIncreaseButtonText.text = $"{hpLevelCost[hpLevel]}$";
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

    public void Temp()
    {
        gameManagerStatic.Temp();
    }


    public void GameEnd(string gameObjectTag)
    {
        currentPanel = checkWin(gameObjectTag);
        StartCoroutine("timeFreezeIEnumEndGame");

        topPanelSwitch(false);

        currentPanel.SetActive(true);
        currentPanel.GetComponent<Animator>().SetBool("Open", true);
    }


    public void PauseSwitcer()
    {
        if (!isPausing)
        {
            isPause = !isPause;
            if (isPause)
                PauseStart();
            else
                PauseResume();
        }
    }

    private void PauseStart()
    {
        isPausing = true;
        StartCoroutine("timeFreezeIEnum");
        topPanelSwitch(false);
        PausePanel.SetActive(true);
        PausePanel.GetComponent<Animator>().SetBool("Open", true);
    }

    private void PauseResume()
    {
        timeRevert();
        topPanelSwitch(true);
        PausePanel.SetActive(false);
        PausePanel.GetComponent<Animator>().SetBool("Open", false);
    }

    private void topPanelSwitch(bool switcher)
    {
        foreach (Transform item in TopPanel.transform)
        {
            if (item.gameObject.GetComponent<Button>())
            {
                item.gameObject.GetComponent<Button>().enabled = switcher;
            }
        }
    }

    private GameObject checkWin(string gameObjectTag)
    {
        if (gameObjectTag == "bot_enemy")
            return DefeatPanel;
        else
            return WinPanel;
    }


    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        timeRevert();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        timeRevert();
    }

    private void timeFreeze()
    {
        Time.timeScale = 0.001f;
        isPausing = false;
    }

    private IEnumerator timeFreezeIEnum()
    {
        isPausing = true;
        while (Time.timeScale > 0.001f)
        {
            Time.timeScale -= 0.01f;
            PausePanelAnim.speed = 1 / Time.timeScale;
            yield return new WaitForSeconds(0.001f * Time.timeScale);
        }
        isPausing = false;
    }

    private IEnumerator timeFreezeIEnumEndGame()
    {
        isPausing = true;
        while (Time.timeScale > 0.001f)
        {
            Time.timeScale -= 0.01f;
            WinPanelAnim.speed = 1 / Time.timeScale;
            DefeatPanelAnim.speed = 1 / Time.timeScale;
            yield return new WaitForSeconds(0.001f * Time.timeScale);
        }
    }

    private void timeRevert()
    {
        Time.timeScale = 1f;
    }

    public void setVolume(float vol)
    {
        gameManagerStatic.Volume = vol;
        AudioListener.volume = vol;
    }


}
