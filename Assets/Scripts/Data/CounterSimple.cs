using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CounterSimple
{
    private bool isTicking;
    private int targetTick;
    private int ticks;
    private Action onComplete;
    private Func<bool> stopAction;
    public CounterSimple(Func<bool> stopAction = null)
    {
        this.stopAction = stopAction;
        WorldTimer.Instance.tick += Tick;
    }
    private void Tick()
    {
        if(!isTicking) return;
        if(stopAction != null && stopAction()) Stop();
        if(++ticks >= targetTick)
        {
            //Debug.Log("complete counter");
            Stop();
            onComplete?.Invoke();
        }
    }   
    public void Start(float duration, Action onComplete)
    {
        this.onComplete = onComplete;
        //Debug.Log("start tick counter");
        int durationInTick = Mathf.RoundToInt(duration / WorldTimer.Instance.TickLengh);
        targetTick = durationInTick > 0 ? durationInTick : 1;
        ticks = 0;
        isTicking = true;
    }
    public void Stop()
    {
        //Debug.Log("stop tick counter");
        ticks = 0;
        isTicking = false;
    }
    public void Reset()
    {
        ticks = 0;
    }

    
}

    

