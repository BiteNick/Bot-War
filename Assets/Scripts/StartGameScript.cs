using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class StartGameScript : MonoBehaviour //responsible for resources and spawn player's units
{
    [SerializeField] private GameObject AkGuyButton;
    [SerializeField] private GameObject AkGuyPrefab;
    [SerializeField] private GameObject AkGuy;
    private float AkGuyCost = 3f;
    private float AkGuyCooldown = 0.5f;


    [SerializeField] private GameObject MachineGunGuyButton;
    [SerializeField] private GameObject MachineGunGuyPrefab;
    [SerializeField] private GameObject MachineGunGuy;
    private float MachineGunGuyCost = 5f;
    private float MachineGunGuyCooldown = 3f;

    [SerializeField] private GameObject SniperGuyButton;
    [SerializeField] private GameObject SniperGuyPrefab;
    [SerializeField] private GameObject SniperGuy;
    private float SniperGuyCost = 6f;
    private float SniperGuyCooldown = 7f;

    [SerializeField] private GameObject ShotGunnerGuyButton;
    [SerializeField] private GameObject ShotGunnerGuyPrefab;
    [SerializeField] private GameObject ShotGunnerGuy;
    private float ShotGunnerGuyCost = 10f;
    private float ShotGunnerGuyCooldown = 2.5f;


    [SerializeField] private Button IncreaseHpButton;
    [SerializeField] private Button IncreaseIncomeButton;

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


    private GameObject UnitsPanel;
    private GameObject DefeatPanel;
    private GameObject WinPanel;
    private GameObject PausePanel;
    private GameObject currentPanel;

    private bool isPause = false;
    private bool isPausing = false; //true if doing pause

    [SerializeField] private Slider VolumeSlider;
    [SerializeField] private Slider CameraKeysMovingSpeedSlider;
    [SerializeField] private Slider CameraMoveSpeedSlider;
    [SerializeField] private Slider CameraZoomSpeedSlider;

    private Animator PausePanelAnim;
    private Animator WinPanelAnim;
    private Animator DefeatPanelAnim;

    [SerializeField] private float PauseSpeed = 2f; //the more the faster doing pause

    private void Awake()
    {

        gameManagerStatic.currentMapWidthX = GameObject.Find("mainTerrain").GetComponent<Terrain>().terrainData.size.x;
        gameManagerStatic.currentMapWidthZ = GameObject.Find("mainTerrain").GetComponent<Terrain>().terrainData.size.z;

        gameManagerStatic.StartManager();
        UnitsPanel = GameObject.FindGameObjectWithTag("UnitsPanel");
        DefeatPanel = GameObject.FindGameObjectWithTag("DefeatPanel");
        DefeatPanel.SetActive(false);
        WinPanel = GameObject.FindGameObjectWithTag("WinPanel");
        WinPanel.SetActive(false);
        PausePanel = GameObject.FindGameObjectWithTag("PausePanel");
        PausePanel.SetActive(false);

        AkGuyPrefab = Resources.Load<GameObject>("prefabs/characters/AllyBot");
        MachineGunGuyPrefab = Resources.Load<GameObject>("prefabs/characters/AllyBotMachineGunner");
        SniperGuyPrefab = Resources.Load<GameObject>("prefabs/characters/AllyBotSniper");
        ShotGunnerGuyPrefab = Resources.Load<GameObject>("prefabs/characters/AllyBotShotGunner");

        levelWaves = Resources.Load<levelWaves>($"ScriptableObjects/levelsSpawn/{SceneManager.GetActiveScene().name}");
    }

    private void Start()
    {
        if (gameManagerStatic.GraphicsQuality == 0)
        {
            PostProcessVolume component = gameObject.GetComponent <PostProcessVolume>();
            component.enabled = false;
        }


        AkGuy = Instantiate(AkGuyPrefab, transform.position, Quaternion.identity);
        gameManagerStatic.botRunStatsAlly.Add(AkGuy.GetComponent<BotRun>());
        AkGuy.SetActive(false);

        MachineGunGuy = Instantiate(MachineGunGuyPrefab, transform.position, Quaternion.identity);
        gameManagerStatic.botRunStatsAlly.Add(MachineGunGuy.GetComponent<BotRun>());
        MachineGunGuy.SetActive(false);

        SniperGuy = Instantiate(SniperGuyPrefab, transform.position, Quaternion.identity);
        gameManagerStatic.botRunStatsAlly.Add(SniperGuy.GetComponent<BotRun>());
        SniperGuy.SetActive(false);

        ShotGunnerGuy = Instantiate(ShotGunnerGuyPrefab, transform.position, Quaternion.identity);
        gameManagerStatic.botRunStatsAlly.Add(ShotGunnerGuy.GetComponent<BotRun>());
        ShotGunnerGuy.SetActive(false);



        PausePanelAnim = PausePanel.GetComponent<Animator>();
        WinPanelAnim = WinPanel.GetComponent<Animator>();
        DefeatPanelAnim = DefeatPanel.GetComponent<Animator>();

        if (VolumeSlider != null)
            VolumeSlider.value = gameManagerStatic.Volume;

        if (CameraKeysMovingSpeedSlider != null)
            CameraKeysMovingSpeedSlider.value = gameManagerStatic.CameraKeysMovingSpeed / gameManagerStatic.CameraMaxKeysMovingSpeed;

        if (CameraMoveSpeedSlider != null)
            CameraMoveSpeedSlider.value = gameManagerStatic.CameraMoveSpeed / gameManagerStatic.CameraMaxMoveSpeed;

        if (CameraZoomSpeedSlider != null)
            CameraZoomSpeedSlider.value = gameManagerStatic.CameraScrollSpeed / gameManagerStatic.CameraMaxScrollSpeed;



        AudioListener.volume = gameManagerStatic.Volume; ;
        IncomeIncreaseButtonText.text = $"{IncomeLevelCost[0]}$";
        hpIncreaseButtonText.text = $"{hpLevelCost[0]}$";
        levelWaves = Instantiate(levelWaves);
        levelWaves.Init();
        StartCoroutine("Waves");
        StartCoroutine("Income");
    }

    public void SpawnUnit(GameObject unit, float cost, GameObject ButtonObject, float cooldown)
    {
        
        if (unit != null && resources >= cost)
        {
            GameObject spawnedUnit = Instantiate(unit, new Vector3(Random.Range(13f, gameManagerStatic.currentMapWidthX - 10), 0f, 3f), Quaternion.identity);
            spawnedUnit.SetActive(true);
            resources -= cost;
            unitsPanelUpdate();
            StartCoroutine(spawnCooldown(ButtonObject, cooldown));
        }
    }


    private IEnumerator spawnCooldown(GameObject ButtonObject, float cooldown)
    {
        float maxCooldown = cooldown;
        cooldown = 0;
        Image img = ButtonObject.GetComponent<Image>();
        Button btn = ButtonObject.GetComponent<Button>();
        img.fillAmount = 0f;
        while (cooldown < maxCooldown)
        {
            btn.interactable = false; //чтобы обновление этой панели не активировало текущую кнопку с перезарядкой
            img.fillAmount = cooldown / maxCooldown;
            cooldown += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
        btn.interactable = true;
        img.fillAmount = 1f;
        unitsPanelUpdate();
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
            unitsPanelUpdate();
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
            ShotGunnerGuy.GetComponent<BotRun>().IncreaseHp();

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
            unitsPanelUpdate();
        }
    }


    private void unitsPanelUpdate()
    {
        ResourcesText.text = $"{resources}(+{income_resources})$";
        if (resources >= AkGuyCost)
            AkGuyButton.GetComponent<Button>().interactable = true;
        else
            AkGuyButton.GetComponent<Button>().interactable = false;

        if (resources >= SniperGuyCost)
            SniperGuyButton.GetComponent<Button>().interactable = true;
        else
            SniperGuyButton.GetComponent<Button>().interactable = false;

        if (resources >= MachineGunGuyCost)
            MachineGunGuyButton.GetComponent<Button>().interactable = true;
        else
            MachineGunGuyButton.GetComponent <Button>().interactable = false;

        if (resources >= ShotGunnerGuyCost)
            ShotGunnerGuyButton.GetComponent<Button>().interactable = true;
        else
            ShotGunnerGuyButton.GetComponent<Button>().interactable = false;


        if (hpLevel < hpLevelCost.Length && resources >= hpLevelCost[hpLevel])
            IncreaseHpButton.GetComponent<Button>().interactable = true;
        else
            IncreaseHpButton.GetComponent<Button>().interactable = false;

        if (IncomeLevel < IncomeLevelCost.Length && resources >= IncomeLevelCost[IncomeLevel])
            IncreaseIncomeButton.GetComponent<Button>().interactable = true;
        else
            IncreaseIncomeButton.GetComponent <Button>().interactable = false;
    }


    private IEnumerator Income()
    {
        while (true)
        {
            resources += income_resources;
            unitsPanelUpdate();
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
        foreach (Transform item in UnitsPanel.transform)
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

    public void NextLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
        timeRevert();
    }


    private IEnumerator timeFreezeIEnum()
    {
        isPausing = true;
        while (Time.timeScale > 0.1f)
        {
            Time.timeScale -= Time.fixedDeltaTime;
            PausePanelAnim.speed = 1 / Time.timeScale;
            yield return new WaitForSeconds(Time.fixedDeltaTime * Time.timeScale / PauseSpeed);
        }
        Time.timeScale = 0.0001f;
        isPausing = false;
    }

    private IEnumerator timeFreezeIEnumEndGame()
    {
        isPausing = true;
        while (Time.timeScale > 0.1f)
        {
            Time.timeScale -= Time.fixedDeltaTime;
            WinPanelAnim.speed = 1 / Time.timeScale;
            DefeatPanelAnim.speed = 1 / Time.timeScale;
            yield return new WaitForSeconds(Time.fixedDeltaTime * Time.timeScale / PauseSpeed);
        }
        Time.timeScale = 0.0001f;

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
