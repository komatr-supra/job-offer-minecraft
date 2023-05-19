using System;
using System.Collections;
using UnityEngine;

//TICKER its just timer for everything...
public class WorldTimer : MonoBehaviour
{
    public static WorldTimer Instance;
    [SerializeField] private float tickLenght = 0.1f; 
    public float TickLengh => tickLenght;
    public Action tick;
    private void Awake() {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        StartCoroutine(TimerCoroutine());
    }
    private IEnumerator TimerCoroutine()
    {
        var wait = new WaitForSeconds(tickLenght);
        while (true)
        {
            tick?.Invoke();
            yield return wait;
        }
        
    }
}
