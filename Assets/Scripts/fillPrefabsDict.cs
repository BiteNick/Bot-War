using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class fillPrefabsDict : MonoBehaviour
{
    void Awake()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources");
        FileInfo[] files = dir.GetFiles("*.prefab");
        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i].ToString());
            string file = files[i].ToString().Substring(files[i].ToString().IndexOf("Assets"));
            GameObject go = Resources.Load<GameObject>("READYMyAK74");
            Instantiate(go, new Vector3(1, 1, 1), Quaternion.identity);
            Debug.Log(go);
            prefabsDictionary.prefabs.Add(i, go);

        }
        
    }
    private void Start()
    {
        for (int i = 0; i < prefabsDictionary.prefabs.Count; i++)
        {
            Debug.Log(prefabsDictionary.prefabs[i]);
            Instantiate(prefabsDictionary.prefabs[i], new Vector3(850, 1, 994), Quaternion.identity);
        }
        foreach (var item in prefabsDictionary.prefabs)
        {
            Debug.Log(item);
        }
    }
}
