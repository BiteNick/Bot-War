using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mine : MonoBehaviour
{
    [SerializeField] private GameObject orePrefab;
    public State currentState;
    void Start()
    {
        StartCoroutine("spawnOre");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    IEnumerator spawnOre()
    {
        while (true)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x - Random.Range(-1f, 1f), transform.position.y + Random.Range(0f, 1f), transform.position.z - Random.Range(-1f, 1f));
            Instantiate(orePrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(3f);
        }
    }
}
