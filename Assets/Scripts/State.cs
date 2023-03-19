using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public bool isFinished;
    public BotRun character;
    public string StateName; //немного дилетантских обзываний переменных

    public virtual void preInit() {}

    public virtual void Init() {}

    public virtual void SetTarget(Transform target) { }

    public abstract void Run();
}
