using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digger
{
    
    private CounterSimple destroyCounter;
    private bool stopFlag;
    public Digger(Action onComplete)
    {
        destroyCounter = new CounterSimple(onComplete, () => stopFlag);
    }
    public void StartDigging(BlocksSO blockData)
    {
        stopFlag = false;
        float miningTime = blockData.minigTime;
        destroyCounter.Start(miningTime);
    }
    public bool StopDigging() => stopFlag = true;

}
