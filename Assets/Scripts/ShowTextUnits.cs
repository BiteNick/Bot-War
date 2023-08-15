using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowTextUnits : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private StartGameScript StartGameScript;

    [SerializeField] private GameObject unitButton;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject unit;
    private BotRun unitScript;
    [SerializeField] private float unitCost;
    [SerializeField] private float unitCooldown;


    private GameObject InfoImage;
    private Animator InfoImageAnim;
    private Text InfoImageText;
    private bool isPointed = false;
    private bool isOpen;
    private float timeToOpenInfo = 0.3f;
    private float timeToOpenInfoLeft;


    private float unitMaxHp;
    private float unitDamage;
    private float unitFireRate;
    private float unitRange;
    private float unitSpeed;
    private int unitMagazineCapacity;


    private void Awake()
    {
        StartGameScript = GameObject.Find("GameManager").GetComponent<StartGameScript>();
        unitButton = gameObject;
    }

    private void Start()
    {
        Button button = unitButton.GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        InfoImage = gameObject.transform.Find("InfoImage").gameObject;
        InfoImageAnim = InfoImage.GetComponent<Animator>();
        InfoImageText = InfoImage.transform.Find("InfoText").GetComponent<Text>();
        timeToOpenInfoLeft = timeToOpenInfo;

        unit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        unitScript = unit.GetComponent<BotRun>();

        unitMaxHp = unitScript.maxhp;
        unitDamage = unitScript.Gun.damage;
        unitFireRate = unitScript.Gun.FireRate;
        unitRange = unitScript.Gun.fireDistance;
        unitSpeed = unitScript.speed;
        unitMagazineCapacity = unitScript.Gun.magazineCapacity;

        Debug.Log(unitScript.maxhp);

        unit.SetActive(false);
    }

    private void OnClick()
    {
        StartGameScript.SpawnUnit(unit, unitCost, unitButton, unitCooldown);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointed = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointed = false;
        isOpen = false;
        timeToOpenInfoLeft = timeToOpenInfo;
        InfoImageAnim.SetBool("isOpen", false);
        Invoke("CloseTabInvoked", 0.6f);
    }

    public void CloseTabInvoked()
    {
        if (!isPointed)
            InfoImage.SetActive(false);
    }

    


    private void Update()
    {
        if (isPointed)
        {
            timeToOpenInfoLeft -= Time.deltaTime;
            if (timeToOpenInfoLeft < 0)
            {
                if (!isOpen)
                    openInfoPage();
                isOpen = true;
            }
        }
    }

    private void openInfoPage()
    {
        InfoImage.SetActive(true);
        InfoImageAnim.SetBool("isOpen", true);
        InfoImageText.text = $"HP: {unitMaxHp}\nDamage: {unitDamage}\nRate of fire: {unitFireRate}\nRange: {unitRange}\nSpeed: {unitSpeed}\nMagazine capacity: {unitMagazineCapacity}";
    }

}
