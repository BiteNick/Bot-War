using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class levelWaves : ScriptableObject
{
    [SerializeField] public int mapWidthX;
    [SerializeField] private MoveTo MoveStateEnemy;
    [SerializeField] private float vavesDelayFloat;
    [SerializeField] private float nextVaveResources; //������� �� ��������� �����
    [SerializeField] private float currentResources; //������� �� ������� �����
    [SerializeField, Header("equalParameters")] private List<GameObject> UnitsList; //������ ��������� ������������ � ������
    [SerializeField, Header("equalParameters")] private List<int> UnitsCost; //������ ��������� ������������ � �������
    [SerializeField] private Dictionary<GameObject, float> unitsCost;
    private int minimalUnitCost = int.MaxValue;
    public WaitForSeconds vavesDelay;

    public void Init()
    {
        vavesDelay = new WaitForSeconds(vavesDelayFloat);
        foreach (int cost in UnitsCost)
        {
            if (cost < minimalUnitCost)
            {
                minimalUnitCost = cost;
            }
        }
    }

    public void Run()
    {
        while (currentResources >= minimalUnitCost)
        {
            int currentUnitSpawn = Random.Range(0, UnitsList.Count);
            Instantiate(UnitsList[currentUnitSpawn], new Vector3(Random.Range(5, mapWidthX - 5), 0f, MoveStateEnemy.mapBeginningZ - 5), Quaternion.identity);
            currentResources -= UnitsCost[currentUnitSpawn];
        }
        currentResources += nextVaveResources;
    }
}

