using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CounterSimple : IDisposable
{
    private bool isTicking;
    private int targetTick;
    private int ticks;
    private Action onComplete;
    public CounterSimple(float duration, Action onComplete)
    {
        this.onComplete = onComplete;
        WorldTimer.Instance.tick += Tick;
        int durationInTick = Mathf.RoundToInt(duration / WorldTimer.Instance.TickLengh);
        targetTick = durationInTick > 0 ? durationInTick : 1;
    }
    private void Tick()
    {
        if(!isTicking) return;
        if(++ticks >= targetTick)
        {
            Stop();
            onComplete?.Invoke();
        }
    }   
    public void Start()
    {
        ticks = 0;
        isTicking = true;
    }
    public void Stop()
    {
        Debug.Log("stop tick counter");
        ticks = 0;
        isTicking = false;
    }
    public void Reset()
    {
        ticks = 0;
    }

    public void Dispose()
    {
        WorldTimer.Instance.tick -= Tick;
    }
}

    

